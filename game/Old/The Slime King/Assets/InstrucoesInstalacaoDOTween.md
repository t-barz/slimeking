# Instruções para instalação do DOTween

Para resolver o erro de referência ao DOTween no projeto The Slime King, siga os passos abaixo:

## Opção 1: Instalar via Package Manager (recomendado)

1. No Unity, abra **Window > Package Manager**
2. Clique no botão **+** no canto superior esquerdo
3. Selecione **Add package from git URL...**
4. Cole o URL: `https://github.com/Demigiant/dotween.git`
5. Clique em **Add**
6. Aguarde o download e importação do pacote

## Opção 2: Baixar e importar manualmente

1. Acesse o site oficial do DOTween: [http://dotween.demigiant.com/download.php](http://dotween.demigiant.com/download.php)
2. Baixe a versão mais recente do DOTween
3. No Unity, vá para **Assets > Import Package > Custom Package...**
4. Navegue até o arquivo baixado e selecione-o
5. Na janela de importação que aparece, certifique-se de que todos os itens estão selecionados
6. Clique em **Import**
7. Quando a importação for concluída, vá para **Tools > Demigiant > DOTween Utility Panel**
8. Clique em **Setup DOTween...**
9. Na janela que aparece, clique em **Apply**

## Opção 3: Adicionar ao Manifest.json

1. Abra o arquivo `Packages/manifest.json` no seu projeto
2. Adicione a seguinte linha na seção "dependencies":
   ```json
   "com.demigiant.dotween": "https://github.com/Demigiant/dotween.git",
   ```
3. Salve o arquivo e retorne ao Unity para que os pacotes sejam atualizados

## Verificação

Após a instalação, verifique se o DOTween está funcionando corretamente:

1. No seu script, verifique se a linha de importação `using DG.Tweening;` não apresenta mais erros
2. Compile o projeto e verifique se não há mais erros relacionados ao DOTween

## Nota importante

Se você estiver usando o Unity 2019.3 ou superior, talvez seja necessário adicionar o Assembly Definition Reference do DOTween ao seu Assembly Definition File (se você estiver usando).

## Links úteis

- [Documentação oficial do DOTween](http://dotween.demigiant.com/documentation.php)
- [Repositório no GitHub](https://github.com/Demigiant/dotween)
