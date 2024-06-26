﻿using System.Data;

namespace ProdutoCatalogo.Shared.Messages;

public static class ValidationMessages
{
    private const string DefaultMissingField = " é obrigatório para essa requisição.";
    private const string DefaultPermittedRange = " fora do intervalo permitido.";
    private const string FailureTo = "Falha ao gerar ";

    public const string DefaultInvalidField = " inválido ou ausente para essa requisição.";
    public const string GenericReturnBadRequest = "A solicitação não pode ser concluída";
    public const string RegisterNotFound = "Registro não localizado.";
    public const string EmptyReturn = "Sem Registros para a solicitação.";
    public const string EmptyQueryParams = "Deve ser informado um campo para pesquisa.";
    public static class Token
    {
        private const string itemName = "Token de Acesso";
        public const string NotGenerate = $"{FailureTo}{itemName}.";
        public const string Expired = $"{itemName} expirado.";
        public const string Invalid = $"{itemName}{DefaultInvalidField}";
    }

    public static class Header
    {
        private const string itemName = "Header: ";
        public static class Authorization
        {
            private const string headerName = "Authorization";
            public const string Missing = $"{itemName}{headerName}{DefaultMissingField}.";
            public const string Invalid = $"{itemName}{headerName}{DefaultInvalidField}.";
            public const string OwnerInvalid = $"{itemName}{headerName} código de autorização inválido";
            public const string RoleInvalid = $"{itemName}{headerName} nível de acesso inválido";
        }
        public static class UserAgent
        {
            private const string headerName = "User-Agent";
            public const string Missing = $"{itemName}{headerName}{DefaultMissingField}.";
        }
        public static class RequestTimestamp
        {
            private const string headerName = "x-request-timestamp";
            public const string Missing = $"{itemName}{headerName}{DefaultMissingField}.";
            public const string Expired = $"{itemName}{headerName}{DefaultPermittedRange}.";
            public const string Invalid = $"{itemName}{headerName}{DefaultInvalidField}.";
        }
    }

    public static class Pagination
    {
        public const string Current = "O número da página deve ser maior que 0.";
        public const string Size = $"O tamanho da página deve estar entre: 10, 50, 100 ou 150.";
    }

    public static class User
    {
        public static class Id
        {
            private const string itemName = "Id";
            public const string Missing = $"{itemName}{DefaultMissingField}";
        }
        public static class Nickname
        {
            private const string itemName = "Apelido";
            public const string Missing = $"{itemName}{DefaultMissingField}";
            public const string Invalid = $"{itemName}{DefaultInvalidField}";
            public const string Size = $"{itemName} deve conter no máximo 50 caracteres";
        }

        public static class Email
        {
            private const string itemName = "E-mail";
            public const string Missing = $"{itemName}{DefaultMissingField}";
            public const string Invalid = $"{itemName}{DefaultInvalidField}";
            public const string Size = $"{itemName} deve conter no máximo 255 caracteres";
        }
        public static class Password
        {
            private const string itemName = "Senha";
            public const string Missing = $"{itemName}{DefaultMissingField}";
            public const string Size = $"{itemName} deve conter no máximo 900 caracteres";
        }
        public static class Permission
        {
            private const string itemName = "IdPermissao";
            public const string Missing = $"{itemName}{DefaultMissingField}";
        }
    }

    public static class Category
    {
        public static class Name
        {
            private const string itemName = "Nome";
            public const string Missing = $"{itemName}{DefaultMissingField}";
            public const string Size = $"{itemName} deve conter no máximo 80 caracteres";
        }
        public static class Id
        {
            private const string itemName = "Id";
            public const string Missing = $"{itemName}{DefaultMissingField}";
        }
    }
    public static class Product
    {
        public static class Name
        {
            private const string itemName = "Nome";
            public const string Missing = $"{itemName}{DefaultMissingField}";
            public const string Size = $"{itemName} deve conter no máximo 100 caracteres";
        }
        public static class Description
        {
            private const string itemName = "Descricao";
            public const string Missing = $"{itemName}{DefaultMissingField}";
            public const string Size = $"{itemName} deve conter no máximo 250 caracteres";
        }
        public static class PurchasePrice
        {
            private const string itemName = "ValorCompra";
            public const string Missing = $"{itemName}{DefaultMissingField}";
        }
        public static class ValuePrice
        {
            private const string itemName = "ValorVenda";
            public const string Missing = $"{itemName}{DefaultMissingField}";
        }
        public static class Category
        {
            private const string itemName = "IdCategoria";
            public const string Missing = $"{itemName}{DefaultMissingField}";
        }
        public static class Id
        {
            private const string itemName = "Id";
            public const string Missing = $"{itemName}{DefaultMissingField}";
        }
    }
}
