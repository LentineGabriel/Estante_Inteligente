import uvicorn
from fastapi import FastAPI, status
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse

from app.api.router import api_router
from app.core.config import settings
from app.core.database import get_connection

# Instancia a aplicação FastAPI definindo o título e versão de forma dinâmica
app = FastAPI(
    title=settings.PROJECT_NAME,
    version=settings.PROJECT_VERSION,
    description="API robusta para gerenciamento de biblioteca e controle pessoal de livros (Estante virtual)."
)

# Configuração do Middleware de CORS (Cross-Origin Resource Sharing)
# Essencial para que o seu front-end futuro consiga se conectar e consumir a API localmente
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Permite requisições de qualquer origem. Pode ser restrito em produção.
    allow_credentials=True,
    allow_methods=["*"],  # Permite todos os métodos HTTP (GET, POST, PUT, DELETE, PATCH, etc.)
    allow_headers=["*"],  # Permite todos os cabeçalhos HTTP nas requisições
)

# Registro do roteador centralizado contendo todos os endpoints
app.include_router(api_router)

@app.get("/", tags=["Root"])
def root():
    """
    Endpoint raiz amigável para checagem rápida de integridade da API.
    """
    return {
        "message": "API da Biblioteca pronta e operacional",
        "status": "healthy",
        "version": settings.PROJECT_VERSION
    }

@app.get("/health", tags=["Observabilidade"])
def health():
    """
    Endpoint robusto de checagem de saúde (healthcheck) de acordo com padrões DevOps.
    Tenta estabelecer conexões de teste com serviços externos dependentes (Banco de Dados).
    """
    db_status = "connected"
    http_status = status.HTTP_200_OK
    
    try:
        # Tenta estabelecer uma conexão rápida e executa um SELECT 1 para teste físico
        conn = get_connection()
        with conn.cursor() as cur:
            cur.execute("SELECT 1;")
            cur.fetchone()
        conn.close()
    except Exception as e:
        # Caso ocorra falha de rede, timeout ou credenciais inválidas do banco
        db_status = f"disconnected: {str(e)}"
        http_status = status.HTTP_503_SERVICE_UNAVAILABLE

    response_content = {
        "status": "healthy" if http_status == status.HTTP_200_OK else "unhealthy",
        "database": db_status,
        "version": settings.PROJECT_VERSION
    }
    
    return JSONResponse(
        status_code=http_status,
        content=response_content
    )

if __name__ == "__main__":
    # Inicializa o servidor web Uvicorn caso este arquivo seja rodado diretamente
    # com recarregamento dinâmico em tempo de execução (--reload)
    uvicorn.run("app.main:app", host="0.0.0.0", port=8000, reload=True)
