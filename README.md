# Fingertech TS Socket

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.5.1-blue)
![License](https://img.shields.io/badge/license-MIT-green)

Serviço TCP Socket para captura remota de impressões digitais utilizando leitores biométricos Nitgen.

## 📋 Descrição

**rdpServiceSocket** é uma aplicação Windows Forms que funciona como um servidor TCP para capturar e registrar impressões digitais através de leitores biométricos Nitgen. O sistema permite integração remota via rede para autenticação biométrica em ambientes corporativos, especialmente útil para comunicação entre máquinas físicas e sessões RDP.

## ✨ Características

- 🌐 Servidor TCP Socket na porta 13000
- 👆 Suporte a leitores biométricos Nitgen
- 📡 Comunicação via protocolo TCP/IP
- 🔄 Dois modos de operação: Captura e Registro (Enroll)
- 💻 Interface gráfica com bandeja do sistema (System Tray)
- 🔍 Monitoramento de requisições e status
- ⚡ Processamento assíncrono em thread separada

## 🔧 Pré-requisitos

- Windows (testado em Windows 7+)
- .NET Framework 4.5.1 ou superior
- Leitor biométrico Nitgen compatível
- SDK NBioBSP da Nitgen instalado
- Arquivo de configuração `fingertechts.ini`

## 📦 Instalação

### 1. Clone ou baixe o projeto

```bash
git clone https://github.com/seu-usuario/rdpServiceSocket.git
cd rdpServiceSocket
```

### 2. Instale o SDK Nitgen

Baixe e instale o **NITGEN NBioBSP SDK** do site oficial da Nitgen.

### 3. Configure o arquivo de IP

Após a instalação, vá para a pasta `C:\Windows\` e crie/edite o arquivo `fingertechts.ini`:

```
192.168.1.100
```

> ⚠️ **Importante**: Altere para o IP real da máquina física onde o leitor está conectado.

### 4. Compile o projeto

Abra a solução no Visual Studio e compile:

```
Build > Build Solution (Ctrl+Shift+B)
```

## 🚀 Uso

### Iniciar o Serviço

1. Execute o aplicativo `rdpServiceSocket.exe`
2. O programa iniciará minimizado na bandeja do sistema
3. Duplo clique no ícone da bandeja para visualizar a interface
4. Verifique se o status exibe "Serviço Iniciado"

### Interface

A janela principal exibe:

- **IP Local**: Endereço IP do servidor
- **IP da última requisição**: Cliente que fez a última solicitação
- **Status**: Estado atual do serviço (iniciado, capturando, erros, etc.)

### Integração via Cliente TCP

O servidor escuta na **porta 13000** e aceita dois comandos:

#### Comando `0` - Registrar Digital (Enroll)

Inicia o processo completo de registro de uma nova impressão digital (geralmente requer múltiplas capturas).

**Exemplo de requisição:**
```
Enviar: "0"
Receber: String TextFIR com a digital registrada
```

#### Comando `1` - Capturar Digital (Verify)

Captura uma única impressão digital para verificação.

**Exemplo de requisição:**
```
Enviar: "1"
Receber: String TextFIR com a digital capturada
```

### Exemplo de Cliente em C#

```csharp
using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main()
    {
        try
        {
            // Conectar ao servidor
            TcpClient client = new TcpClient("192.168.1.100", 13000);
            NetworkStream stream = client.GetStream();

            // Enviar comando (0 = Enroll, 1 = Capture)
            byte[] comando = Encoding.ASCII.GetBytes("1");
            stream.Write(comando, 0, comando.Length);

            // Receber resposta
            byte[] buffer = new byte[10000];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            string digital = Encoding.ASCII.GetString(buffer, 0, bytes);

            // Fechar conexão
            client.Close();

            // Exibir resultado
            if (string.IsNullOrEmpty(digital))
            {
                Console.WriteLine("Erro: Dispositivo não encontrado ou falha na captura");
            }
            else
            {
                Console.WriteLine($"Digital capturada com sucesso!");
                Console.WriteLine($"TextFIR: {digital}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
```

### Exemplo de Cliente em Python

```python
import socket

def capturar_digital(ip, porta=13000, comando="1"):
    """
    Captura digital via socket TCP
    
    Args:
        ip: IP do servidor
        porta: Porta do servidor (padrão: 13000)
        comando: "0" para Enroll, "1" para Capture
    
    Returns:
        String com a digital em formato TextFIR
    """
    try:
        # Conectar ao servidor
        client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        client.connect((ip, porta))
        
        # Enviar comando
        client.send(comando.encode('ascii'))
        
        # Receber resposta
        digital = client.recv(10000).decode('ascii')
        
        # Fechar conexão
        client.close()
        
        return digital if digital else None
        
    except Exception as e:
        print(f"Erro: {e}")
        return None

# Uso
digital = capturar_digital("192.168.1.100", comando="1")
if digital:
    print(f"Digital capturada: {digital}")
else:
    print("Falha na captura")
```

## 📡 Protocolo de Comunicação

### Fluxo de Operação

```
Cliente                    Servidor                  Leitor Nitgen
  |                           |                            |
  |--- Conectar TCP:13000 --->|                            |
  |                           |                            |
  |--- Enviar "0" ou "1" ---->|                            |
  |                           |                            |
  |                           |--- Solicitar captura ----->|
  |                           |                            |
  |                           |<-- Aguardar digital -------|
  |                           |    (usuário coloca dedo)   |
  |                           |                            |
  |<-- TextFIR ou "" ---------|<-- Digital capturada ------|
  |                           |                            |
  |--- Desconectar ---------->|                            |
```

### Formato de Resposta

- **Sucesso**: String TextFIR (formato proprietário Nitgen)
- **Falha**: String vazia `""`

## 🛠️ Estrutura do Projeto

```
rdpServiceSocket/
├── Form1.cs                    # Interface e servidor TCP
├── Form1.Designer.cs           # Design da interface (auto-gerado)
├── utilsNitgen.cs              # Classe utilitária para SDK Nitgen
├── Program.cs                  # Ponto de entrada da aplicação
├── App.config                  # Configurações do .NET Framework
├── Properties/
│   ├── AssemblyInfo.cs         # Metadados do assembly
│   ├── Resources.Designer.cs   # Recursos embarcados
│   └── Settings.Designer.cs    # Configurações da aplicação
└── rdpServiceSocket.csproj     # Arquivo do projeto
```

## 📝 Classes Principais

### `Form1`
Classe principal que implementa:
- Interface gráfica Windows Forms
- Servidor TCP Socket
- Gerenciamento de threads
- Comunicação com clientes

### `utilsNitgen`
Classe utilitária com métodos:

#### `Capturar()`
```csharp
public string Capturar()
```
Captura uma única impressão digital para verificação.

**Retorno**: String TextFIR ou `null` se falhar

#### `Enroll()`
```csharp
public string Enroll()
```
Registra uma nova impressão digital (processo completo).

**Retorno**: String TextFIR ou `null` se falhar

## ⚙️ Configuração

### Arquivo fingertechts.ini

Localização: `C:\Windows\fingertechts.ini`

Conteúdo: IP local em formato IPv4

```
192.168.1.100
```

### Porta TCP

Porta padrão: **13000**

> 💡 Para alterar a porta, modifique a variável `port` no método `Server()` em `Form1.cs`

## 🔒 Segurança

### Considerações Importantes

⚠️ **Este projeto não implementa criptografia!** 

Para uso em produção, considere:

- Implementar SSL/TLS para comunicação segura
- Adicionar autenticação de clientes
- Validar e sanitizar entrada de dados
- Implementar rate limiting para prevenir abuso
- Usar firewall para restringir acesso à porta 13000
- Armazenar templates biométricos de forma segura

## 🐛 Resolução de Problemas

### "Não foi possível encontrar fingertechts.ini"

**Solução**: Criar o arquivo `C:\Windows\fingertechts.ini` com o IP da máquina.

### "Não foi possível Capturar digital, Dispositivo não encontrado"

**Causas possíveis**:
- Leitor biométrico não conectado
- Driver do leitor não instalado
- SDK NBioBSP não instalado corretamente
- Leitor em uso por outra aplicação

**Solução**: 
1. Verifique se o leitor está conectado
2. Reinstale o driver do dispositivo
3. Teste com o software de demonstração da Nitgen

### Servidor não inicia

**Causas possíveis**:
- Porta 13000 já em uso
- Permissões insuficientes
- Firewall bloqueando a porta

**Solução**:
1. Verifique se outra aplicação usa a porta 13000
2. Execute como Administrador
3. Configure exceção no firewall

## 👥 Autores

- Desenvolvedor original - Copyright © 2019

---

**Nota**: Este software requer hardware específico (leitores Nitgen) e SDK proprietário para funcionar corretamente.
