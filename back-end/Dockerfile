# ==========================================
# STAGE 1: Builder (Estágio de Compilação)
# ==========================================
FROM python:3.11-slim AS builder

WORKDIR /build

# Evita arquivos temporários e lixo no build
ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1

# Instala compiladores e pacotes de build necessários
RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    libpq-dev \
    && rm -rf /var/lib/apt/lists/*

# Cria e ativa um ambiente virtual limpo para empacotar dependências
RUN python -m venv /opt/venv
ENV PATH="/opt/venv/bin:$PATH"

COPY requirements.txt .
RUN pip install --no-cache-dir --upgrade pip \
    && pip install --no-cache-dir -r requirements.txt


# ==========================================
# STAGE 2: Runner (Estágio de Execução)
# ==========================================
FROM python:3.11-slim AS runner

WORKDIR /app

ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1
ENV PATH="/opt/venv/bin:$PATH"

# Instala curl para o healthcheck nativo e libpq (necessário para psycopg)
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    libpq5 \
    && rm -rf /var/lib/apt/lists/*

# Copia apenas o virtual environment contendo dependências prontas do builder
COPY --from=builder /opt/venv /opt/venv

# Cria um grupo e um usuário de sistema com privilégios limitados
RUN groupadd -g 1000 appgroup \
    && useradd -u 1000 -g appgroup -m -s /bin/bash appuser

# Copia o código do app definindo a propriedade para o appuser
COPY --chown=appuser:appgroup . .

# Garante propriedade do diretório de trabalho ao appuser
RUN chown -R appuser:appgroup /app

# Executa estritamente sob o usuário restrito de sistema
USER appuser

# Expõe a porta 8000
EXPOSE 8000

# Healthcheck nativo no Docker que monitora a saúde a cada 30 segundos
HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8000/health || exit 1

# Inicializa a aplicação FastAPI com Uvicorn
CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]
