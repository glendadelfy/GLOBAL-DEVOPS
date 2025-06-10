# PROJETO AbrigoAPIMinimal .NET API
GLENDA DELFY VELA MAMANI – RM 552667 LUCAS ALCÂNTARA CARVALHO – RM 95111 RENAN BEZERRA DOS SANTOS – RM 553228
# AbrigooAPI Minimal 

## 📖 Descrição
A AbrigoAPI Minimal é uma API desenvolvida para a gestão de abrigos temporários, proporcionando maior controle sobre ocupação, infraestrutura e emergências. Este sistema integra Machine Learning 
para prever a capacidade dos abrigos, JWT para segurança e Swagger para documentação interativa.

## ⚙️ Tecnologias Utilizadas
- 🔹 ASP.NET Minimal API
- 🔹 Entity Framework Core 8
- 🔹 Microsoft ML.NET
- 🔹 JWT Autenticação
- 🔹 Testes de integração Xunit
  
## 🛠️ Funcionalidades
✔ Cadastro de abrigos, pessoas, recursos e eventos
✔ Previsão de ocupação dos abrigos via Machine Learning 
✔ Proteção dos dados via JWT 
✔ Swagger para documentação interativa 
✔ Testes automatizados com Xunit

## 🚀 Como Executar
```bash
# Clone o repositório
https://github.com/glendadelfy/GLOBAL-DOTNET.git

# Acesse a pasta do projeto
cd AbrigoAPIMinimal

# Instale as dependências
dotnet restore

# Execute a API
dotnet run

# Antes de acessar endpoints protegidos (POST), gere um token JWT
POST /gerar-token

# Para treinar o modelo de ocupação de abrigos
POST /capacidade-abrigo/treinar-modelo

# Após o modelo treinado, use o endpoint de previsão de ocupação
POST /capacidade-abrigo/prever

## 🚀 Como Executar os Testes Autonamizados
A AbrigoAPI Minimal possui testes automatizados escritos em Xunit,
garantindo estabilidade e confiabilidade das funcionalidades implementadas. Os testes cobrem áreas essenciais como:
✔ Autenticação JWT → Garante que tokens JWT são gerados corretamente e protegem endpoints.
✔ TesteAbrigoEndpoints.cs → Cobertura completa das operações de Abrigos.
✔ TestePessoaEndpoints.cs → Valida operações relacionadas às Pessoas.
✔ TesteRecursoEndpoints.cs → Assegura o gerenciamento correto dos Recursos.
✔ TesteEventoEndpoints.cs → Testa o gerenciamento de eventos nos abrigos.

### 📌 **Passo a Passo para Executar os Testes**
1️⃣ **Certifique-se de que as dependências do projeto estão instaladas**
```bash
# Certifique-se de que as dependências do projeto estão instaladas
dotnet restore

# Execute os testes no terminal
dotnet test
### 📌 Exemplos de Requisições JSON

Mostra todos os abrigos disponiveis
GET /abrigos/disponiveis

Busca pelo ID
GET /abrigos/buscar/19
JSON
{
  "id": 19,
  "nome": "Abrigo Esperança",
  "localizacao": "Rua das Flores, 123 - São Paulo, SP",
  "capacidade": 50,
  "moradores": null
}

POST /abrigos/criar
JSON
{
  "Id": 1,
  "Nome": "Abrigo Esperança",
  "Localizacao": "Rua das Flores, 123 - São Paulo, SP",
  "Capacidade": 50,
  "Moradores": [
    {
      "Id": 101,
      "Nome": "Maria Souza",
      "Idade": 35,
      "Genero": "Feminino",
      "NecessidadesEspeciais": false,
      "AbrigoId": 1
    },
    {
      "Id": 102,
      "Nome": "Carlos Pereira",
      "Idade": 42,
      "Genero": "Masculino",
      "NecessidadesEspeciais": true,
      "AbrigoId": 1
    }
  ]
}
Atualiza pelo ID
PUT /abrigos/atualizar/19
{
  "Id": 1,
  "Nome": "Abrigo Esperança",
  "Localizacao": "Rua das Flores, 123 - São Paulo, SP",
  "Capacidade": 50,
  "Moradores": [
    {
      "Id": 101,
      "Nome": "Maria Souza",
      "Idade": 35,
      "Genero": "Feminino",
      "NecessidadesEspeciais": false,
      "AbrigoId": 1
    },
    {
      "Id": 102,
      "Nome": "Carlos Almeida",
      "Idade": 42,
      "Genero": "Masculino",
      "NecessidadesEspeciais": true,
      "AbrigoId": 1
    }
  ]
}
Deleta pelo ID
DELETE /abrigos/deletar/15





