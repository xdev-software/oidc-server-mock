# 1.0.3
* Update ``YamlDotNet`` to latest version

# 1.0.2
* Fix ``arm64`` build

# 1.0.0
_Initial release_

Based on: [Soluto/oidc-server-mock:0.9.2](https://github.com/Soluto/oidc-server-mock/releases/tag/v0.9.2)

Contains the following differences to [v0.9.2 of Soluto/oidc-server-mock](https://github.com/Soluto/oidc-server-mock/releases/tag/v0.9.2):
* Image improvements
  * Actually works on ``arm64``
  * uses ``alpine`` instead of ``debian/ubunut`` and is therefore a lot smaller
  * uses ZSTD for compression
    * extraction is a lot faster
    * image is a bit smaller
* Updated dependencies
* Also released to DockerHub
