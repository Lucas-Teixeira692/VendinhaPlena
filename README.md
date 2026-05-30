# Vendinha Plena - Sistema de Controlo de Fiado

O **Vendinha Plena** é uma aplicação completa para gestão de clientes e controlo de dívidas (fiado), desenvolvida com as melhores práticas de engenharia de software do ecossistema .NET. O projeto utiliza uma abordagem de **Arquitetura Limpa (Clean Architecture)** e possui uma interface gráfica nativa em **Windows Forms** construída de forma 100% programática, permitindo a sua execução e desenvolvimento fluido tanto no **Visual Studio** quanto no **VS Code**.

---

## 🚀 Tecnologias Utilizadas

* **Ambiente de Execução:** .NET 8.0
* **Interface Gráfica:** Windows Forms (WinForms) - UI Programática
* **Mapeamento Objeto-Relacional:** Entity Framework Core 8.0
* **Banco de Dados:** SQLite (Auto-gerido)

---

## 🏛️ Arquitetura do Projeto (Clean Architecture)

A solução está dividida em camadas lógicas para garantir o desacoplamento, a testabilidade e a facilidade de manutenção:

1.  **`VendinhaPlena.Domain`:** O coração da aplicação. Contém as entidades de negócio (`Cliente`, `Divida`) e as regras essenciais.
2.  **`VendinhaPlena.Application`:** Camada de casos de uso. Contém os serviços (`ClienteService`) que orquestram a lógica, validações rigorosas (como validação de CPF) e paginação de dados.
3.  **`VendinhaPlena.Infrastructure`:** Responsável pelo acesso a dados. Contém o `VendinhaDbContext`, configurações do EF Core e o repositório de dados.
4.  **`VendinhaPlena.WinForms`:** Camada de apresentação. Uma interface gráfica rica e responsiva com abas separadas para Clientes e Dívidas.

---

## 🌟 Recursos Principais

* **Gestão de Clientes:** Registo completo com validação automática de CPF, data de nascimento e e-mail.
* **Controlo de Dívidas:** Lançamento de novos débitos ("pendurar conta") e consulta do estado de pagamento em tempo real.
* **Baixa Automatizada:** Atualização do estado da dívida de "EM ABERTO" para "PAGA" com registo da data e hora exatas do pagamento.
* **Banco de Dados Inteligente:** Utiliza o mecanismo `EnsureCreated()` do EF Core. Se o ficheiro `vendinha.db` não existir, a aplicação cria o banco de dados e as tabelas estruturadas de forma totalmente transparente no primeiro arranque.
* **Compatibilidade Cruzada de IDEs:** Desenvolvido de forma programática (sem dependência exclusiva do Designer gráfico por clique-e-arraste), o projeto pode ser modificado e executado sem limitações em qualquer editor de código.

---

## 🔧 Como Executar o Projeto

Independentemente do seu ambiente de desenvolvimento de eleição, certifique-se de que tem o **SDK do .NET 8** instalado na sua máquina.

### Opção A: Execução via VS Code (ou Terminal)

1. Abra o terminal na pasta raiz da solução (`VendinhaPlena`).
2. Execute o seguinte comando para compilar e rodar a interface gráfica:
   ```bash
   dotnet run --project VendinhaPlena.WinForms