<p align="center">
  <a href="https://github.com/marlonconcenza" target="_blank"><img alt="I am Marlon Concenza" src="https://img.shields.io/badge/I%20am-Marlon_Concenza-informational"></a>
  <a href="https://github.com/marlonconcenza" target="_blank" ><img alt="Github" src="https://img.shields.io/badge/Github--%23F8952D?style=social&logo=github"></a>
  <a href="https://www.linkedin.com/in/marlon-martins-concenza-53738978" target="_blank" ><img alt="LinkedIn" src="https://img.shields.io/badge/Linkedin--%23F8952D?style=social&logo=linkedin"></a>
  <a href="mailto:marlon.concenza@gmail.com" target="_blank" ><img alt="Email" src="https://img.shields.io/badge/Email--%23F8952D?style=social&logo=gmail"></a>
</p>

## Sobre este projeto

Projeto para aplicar conceitos de utilização [RabbitMQ](https://www.rabbitmq.com) e [MongoDB](https://www.mongodb.com/pt-br).<br /><br />
O projeto é composto por uma API Rest que inclui dados em uma fila do Rabbitmq e um Worker que fica monitorando a fila e salvando os dados no MongoDB.<br /><br />
Foi utilizado Docker Compose para orquestração dos containers, com isso ao subir o projeto teremos quartro containers executando: rabbitmq, mongodb, api e consumer.

## Tecnologias utilizadas

- [Dotnet 5.0](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com)
- [RabbitMQ](https://www.rabbitmq.com)
- [MongoDB](https://www.mongodb.com/pt-br)

## Configuração do ambiente de desenvolvimento

### Pré-requisitos

Para executar esse projeto no modo de desenvolvimento, você precisará ter o dotnet 5.0 e o docker desktop instalado.

### Clonando o repositório

```bash
$ git clone https://github.com/marlonconcenza/rabbitmq.git
```

## Scripts disponíveis

No diretório do seu projeto, você pode rodar:

### `docker-compose up`

Executa o projeto em modo de desenvolvimento.
