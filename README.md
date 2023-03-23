
# Proposta para Serviço de Gestão de Fluxo de Caixa


## Introdução

Com o objetivo de ajudar um comerciante a gerenciar seu fluxo de caixa diário e acompanhar suas transações de débito e crédito, propomos um Serviço de Gestão de Fluxo de Caixa. Este serviço permitirá que o comerciante insira suas transações e gere um relatório diário consolidado de saldo.


## Descrição da Solução

Nosso Serviço de Gestão de Fluxo de Caixa fornecerá as seguintes funcionalidades:



* Gestão de Transações: O serviço permitirá que o comerciante insira suas transações diárias, incluindo débitos e créditos.
* Relatório Diário Consolidado: O serviço gerará um relatório diário consolidado de saldo, mostrando as transações diárias e saldo do comerciante.


## Requisitos Técnicos



* Design da Solução: Projetaremos uma solução técnica que forneça uma explicação não técnica da arquitetura e seu funcionamento.
* Linguagem de Implementação: Podemos usar qualquer linguagem que o comerciante preferir.
* Melhores Práticas: Adotaremos as melhores práticas do setor, incluindo Padrões de Design, Padrões de Arquitetura, Princípios SOLID, etc.
* Leia-me: Forneceremos um readme abrangente com instruções sobre como instalar a aplicação localmente, em um contêiner e como usar os serviços.
* Hospedagem Pública: Hospedaremos a aplicação em um repositório público como o GitHub.


## Design da Solução

Para o proposto Serviço de Gerenciamento de Fluxo de Caixa, usaremos uma combinação de SQL Server, AWS e C# para fornecer uma solução escalável e confiável.


## Visão Geral da Arquitetura

A solução consistirá em três componentes principais:



1. Frontend: uma aplicação baseada na web que permitirá ao comerciante inserir suas transações diárias.
2. Backend: um conjunto de APIs escritas em C# que lidarão com as transações do comerciante e gerarão relatórios diários consolidados.


# Detalhes técnicos


## Frontend

O frontend será uma aplicação baseada na web que permitirá que o comerciante insira suas transações diárias. Usaremos uma combinação de HTML, CSS e JavaScript para construir o frontend. Também utilizaremos um framework de frontend como o Angular ou React para fornecer uma interface responsiva e amigável ao usuário.


## Backend

O backend consistirá em um conjunto de APIs escritas em C#. Essas APIs lidarão com as transações do comerciante e gerarão relatórios diários consolidados. Utilizaremos uma arquitetura de microsserviços para garantir escalabilidade e flexibilidade. Também usaremos Funções Lambda para fornecer computação sem servidor e garantir eficiência de custo.


## Banco de dados

O banco de dados será um DynamoDB hospedado na AWS. Usaremos o DynamoDB para fornecer uma solução de banco de dados escalável e confiável. Também usaremos o Amazon S3 para armazenar quaisquer arquivos ou documentos associados às transações do comerciante.


## Integração

O frontend e o backend se comunicarão por meio de APIs REST. As APIs do backend armazenarão as transações do comerciante no DynamoDB. As APIs do backend também gerarão relatórios diários consolidados, que serão armazenados no Amazon S3. O frontend recuperará os relatórios diários do S3 e os exibirá para o comerciante.


## Implantação

A solução será implantada na AWS. Usaremos o S3 para hospedar os arquivos estáticos do frontend e as APIs do backend. Também usaremos o Amazon para hospedar o DynamoDB. O Amazon S3 será usado para armazenar quaisquer arquivos ou documentos associados às transações do comerciante.


## 


# API de backend

O serviço proposto de Gerenciamento de Fluxo de Caixa fornecerá um conjunto de APIs para lidar com as transações do comerciante e gerar relatórios diários consolidados. Aqui estão os detalhes dos endpoints que forneceremos para o MVP:



* POST /publish

    Este endpoint permitirá que o comerciante adicione, atualize, ou delete uma transação. O corpo da solicitação deve conter uma mensagem com os detalhes da transação, como o tipo de transação (débito ou crédito), valor e descrição. O endpoint retornará o ID da solicitaçao. Ao ser concluído, o cliente subscriber receberá uma notificação com este mesmo ID contendo o resultado da operação.



## Integração

A solução fornece um conjunto de APIs para lidar com as transações do comerciante e gerar relatórios diários consolidados. Esses endpoints permitirão ao comerciante gerenciar seu fluxo de caixa diário de forma eficaz. Ao usar essas APIs, outros sistemas poderão adicionar, recuperar, atualizar e excluir transações, além de recuperar relatórios diários consolidados para datas específicas.


## 


# Visao Panoramica



<p id="gdcalert1" ><span style="color: red; font-weight: bold">>>>>>  gd2md-html alert: inline image link here (to images/image1.png). Store image on your image server and adjust path/filename/extension if necessary. </span><br>(<a href="#">Back to top</a>)(<a href="#gdcalert2">Next alert</a>)<br><span style="color: red; font-weight: bold">>>>>> </span></p>


![alt_text](images/image1.png "image_tooltip")



# Microserviços \


<p id="gdcalert2" ><span style="color: red; font-weight: bold">>>>>>  gd2md-html alert: inline image link here (to images/image2.png). Store image on your image server and adjust path/filename/extension if necessary. </span><br>(<a href="#">Back to top</a>)(<a href="#gdcalert3">Next alert</a>)<br><span style="color: red; font-weight: bold">>>>>> </span></p>


![alt_text](images/image2.png "image_tooltip")



# Entregáveis


## Introduçao

Este projeto foi proposto como parte de um processo seletivo para o cargo de arquitetura com o objetivo de modernizar aplicações monolíticas e implementar boas práticas de resiliência, segurança, performance, manutenibilidade e preparação para o futuro. O projeto desenvolvido trata das transações diárias de um vendedor que precisa gerir seu fluxo de caixa e seu fechamento diário.

Embora nem todos os entregáveis solicitados tenham sido disponibilizados para esta avaliaçao, dependendo de um trade-off, a arquitetura nem sempre contempla todas as atividades em um primeiro momento, variando muito de acordo com a visao de curto, medio e longo prazo do arquiteto. Este trabalho foi desenvolvido para exemplificar as habilidades necessárias para exercer o cargo de arquiteto. As atividades mais relevantes deste projeto foram elaboradas de acordo com as exigências do cargo.

Como todo trabalho de arquitetura, foram desenvolvidas metodologias e recursos-base para que o projeto possa ser entregue aos times de desenvolvimento, utilizando práticas comuns, mas seguindo padrões estabelecidos. Esses artefatos desenvolvidos podem ser escalonados e distribuídos de forma independente em microserviços em uma arquitetura baseada em eventos de uma plataforma sem servidor, sem afetar o esforço, o prazo, a clareza de código e o nível de complexidade de manutenção do time de desenvolvimento.

Foram adotadas como premissas para a nova solução que, por ter escopo reduzido, poderá sofrer muitas alterações e deve ser altamente escalável para aceitar novas funcionalidades de forma simples e com baixo impacto no time de desenvolvimento e nos usuários.

A partir deste projeto entregue, é possível implantar novas funcionalidades e serviços sem prejuízos aos usuários, garantindo um uptime extremamente alto, mesmo diante de cenários catastróficos, devido à natureza robusta de uma aplicação serverless baseada em eventos.

<span style="text-decoration:underline;">Nota:</span> Embora haja características de Over Engineering, este teste entregue também é uma base da proposta para modernizar a aplicação monolítica atual, mencionada na entrevista, considerando experiências similares adquiridas em outros projetos.


## LGPD / Privacidade

Adotamos medidas de segurança rigorosas para garantir a privacidade e proteção de dados pessoais em conformidade com a LGPD. Além disso, implementamos as melhores práticas de monitoramento e logging, permitindo que a equipe de operações possa identificar e solucionar problemas rapidamente, sem comprometer a privacidade e proteção de dados pessoais em uso na aplicação no momento da inspeção.


## Testes unitários

Foram desenvolvidos como uma das práticas para garantir a qualidade do código produzido. Os testes unitários são um tipo de teste automatizado que verifica o comportamento de uma unidade de código, como uma função ou um método, isoladamente do restante do sistema. Eles são importantes para garantir que cada parte do sistema está funcionando corretamente e em conformidade com os requisitos estabelecidos.


## Testes blackbox

Juntamente com os testes unitários, BlackBoxes garantem que a integração interna dos módulos desenvolvidos esteja adequada antes de realizar testes de integração. Este teste testa as unidades de código como um todo. Testes blackbox devem ser executados automaticamente para identificar se o desenvolvimento está garantindo um comportamento ponta-a-ponta adequado, sem estar suscetível a intempéries de disponibilidade dos microserviços, como no caso dos testes de integração. Estes testes podem ser utilizados pelas pipelines de build para evitar que as políticas de checkin e cobertura de código definidas sejam impactadas quando um serviço externo está inalcançável.


## Cobertura de código 

Para assegurar a qualidade dos testes em novos desenvolvimentos, foi definida uma cobertura mínima de código de 80%. Isso significa que, pelo menos, 80% do código precisa ser testado para garantir que a aplicação tenha um nível adequado de cobertura de testes. Dessa forma, a equipe pode ter mais confiança no software e garantir que possíveis erros sejam identificados precocemente, reduzindo os custos e tempo necessários para correções.

A cobertura de código será avaliada automaticamente pela pipeline a cada entrega de código no source control. Caso a cobertura seja inferior a 80%, a entrega do código será recusada e o desenvolvedor e seu lead serão notificados.


## Pipelines de CI/CD 

A pipeline de CI/CD implementada neste projeto foi configurada para ser multi-ambiente, multi-projeto, flexível e segura, garantindo que o processo de entrega de funcionalidades esteja aderindo às boas práticas estabelecidas. Cada ambiente (DEV, UAT, PRD) possui grupos de variáveis para tornar a pipeline genérica e fácil de ser utilizada com outros ambientes. Além disso, a pipeline utiliza o Azure Vault para armazenar os segredos, que são obtidos em tempo de execução da pipeline e inseridos automaticamente no appsettings da solução, eliminando a necessidade de intervenção manual, exposição dos segredos e o uso de ferramentas de terceiros para a tarefa. Essa abordagem garante maior segurança e simplicidade no processo de entrega de funcionalidades.


## Branching

Para gerenciar o desenvolvimento dos microserviços e garantir a qualidade do código, foi estabelecido o uso do Gitflow. Além disso, foram adicionadas algumas adaptações específicas da solução, como a implementação de serverless, testes unitários, testes blackbox e cobertura de código. Essas políticas de branching garantem um fluxo de trabalho mais organizado e eficiente para o desenvolvimento e entrega dos serviços.


## Checkin

Para manter a qualidade e integridade do código, foi estabelecido que o checkin do código só é permitido nas branches feature e hotfix. Dessa forma, evita-se a mistura de código não testado ou incompleto com a versão principal do software. As branches de feature permitem que os desenvolvedores trabalhem em novas funcionalidades sem afetar a versão principal do software, enquanto as branches de hotfix são utilizadas para corrigir problemas críticos na versão atual. Essas políticas de checkin garantem a qualidade do código e minimizam possíveis conflitos durante o processo de integração contínua.


## Testes

Neste projeto, serão adotadas as seguintes políticas de testes:



* Testes unitários e testes blackbox serão executados automaticamente após a conclusão do desenvolvimento das funcionalidades.
* Testes unitários irão garantir que o sistema está confiável de forma modular e agilizam a identificação de falhas técnicas.
* Testes blackbox serão implementados sem a consulta a dados externos, para identificar se o desenvolvimento está garantindo um comportamento adequado de ponta a ponta. Para isso, todos os dados externos devem ser mockados conforme a estrutura fornecida na solução.


## Microservicos

Microserviços são uma parte importante da arquitetura baseada em eventos e, como tal, precisam ser desenvolvidos com padrões e consistência em mente. A previsibilidade é crucial, pois os microserviços podem adicionar camadas extras de complexidade à arquitetura, e a falta de previsibilidade pode levar a problemas de escalabilidade e desempenho. Uma forma de padronizar o comportamento dos microserviços é definir padrões para os códigos de status HTTP que eles retornam. Isso ajuda a garantir que os desenvolvedores de outros serviços possam lidar com as respostas de maneira consistente e previsível. 


#### Respostas esperadas



* 200 - Ok: indica que a solicitação foi bem-sucedida e o conteúdo da resposta é o esperado.
* 400 - Bad Request: indica que a solicitação do cliente é inválida ou malformada e não pode ser processada pelo servidor.
* 401 - Unauthorized: indica que o cliente não está autenticado e não tem permissão para acessar o recurso solicitado.
* 403 - Forbidden: indica que o cliente não tem permissão para acessar o recurso solicitado, mesmo que esteja autenticado.
* 500 - Internal Server Error: indica que ocorreu um erro interno no servidor ao processar a solicitação.

Além disso, outra forma de garantir a padronização dos objetos que os microserviços retornam, é torná-los empacotados de forma padronizada para garantir a consistência.




## Padrões de desenvolvimento

Para o desenvolvimento desta solução, foi utilizado DRY, YAGNI, SOLID, SAGA. É importante mencionar que esta solução irá beneficiar-se de um design modular e escalável, seguindo as melhores práticas de arquitetura, como a separação de preocupações (SoC), que permite que cada módulo da aplicação tenha uma única responsabilidade bem definida. Além disso, a solução foi construída utilizando padrões de design de software que favorecem a reutilização de código, a simplicidade, a legibilidade e a manutenção, como o padrão de Injeção de Dependências e o padrão de Repositório.

Para garantir a segurança da aplicação, foram implementadas práticas de segurança de software, como a validação de entrada de dados, autenticação e autorização, e criptografia de dados sensíveis. Além disso, foram adotadas as melhores práticas de monitoramento e logging, permitindo que a equipe de operações possa identificar e solucionar problemas rapidamente sem comprometer os dados pessoais em curso na aplicação no momento da inspeção.

Por fim, é importante destacar que a solução foi construída com base em uma arquitetura serverless baseada em eventos, que permite que a aplicação seja escalável, tolerante a falhas e altamente disponível. Isso significa que a solução pode ser facilmente escalada e gerenciada em uma infraestrutura de nuvem, proporcionando um ambiente de desenvolvimento e implantação mais eficiente, ágil e seguro.


# Priorizaçao MoSCoW



<p id="gdcalert3" ><span style="color: red; font-weight: bold">>>>>>  gd2md-html alert: inline image link here (to images/image3.png). Store image on your image server and adjust path/filename/extension if necessary. </span><br>(<a href="#">Back to top</a>)(<a href="#gdcalert4">Next alert</a>)<br><span style="color: red; font-weight: bold">>>>>> </span></p>


![alt_text](images/image3.png "image_tooltip")



# Matriz de Prioridade



<p id="gdcalert4" ><span style="color: red; font-weight: bold">>>>>>  gd2md-html alert: inline image link here (to images/image4.png). Store image on your image server and adjust path/filename/extension if necessary. </span><br>(<a href="#">Back to top</a>)(<a href="#gdcalert5">Next alert</a>)<br><span style="color: red; font-weight: bold">>>>>> </span></p>


![alt_text](images/image4.png "image_tooltip")

