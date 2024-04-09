
# Web-Api-ASP.NET

Projeto em ASP.NET para exemplificar o consumo de uma Web API REST


## Instalação do Banco de Dados
Instale o MySQL (neste projeto foi utilizado a versão 8.0.36)
Utilizando as credenciais:
```bash
Usuário: root
Senha: P1x30n#@ndR&
```

Após a instalação execute o script:
```bash
Script_Banco_Bd_Produto_Catalogo.sql
``` 




    
## Rodando localmente

Clone o projeto

```bash
  git clone https://github.com/andreluiz1906/Web-Api-ASP.NET.git
```

Entre no diretório do projeto

```bash
  cd Web-Api-ASP.NET\ProdutoCatalogo.Application
```

Instale as dependências

```bash
  dotnet restore
```

Inicie o servidor

```bash
  dotnet run
```



## Inicialização do Swagger
Após iniciar o servidor, observe a porta em que o servidor está ouvindo:
```bash
  info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5136
```

Inicie o navegador em http://localhost:5136/swagger/index.html
## Importação da Collection no Postman
Abra o Postman, no menu superior esquerdo selecione a opção File > Import (Atalho Ctrl + O), localize o arquivo:
```bash
Catálogo de Produtos - API.postman_collection.json
```
## Autor

- [@andreluiz1906](https://www.github.com/andreluiz1906)

