<h1 align="center">Jellyfin TvHeadend Client Plugin</h1>

<p align="center">
<img alt="Plugin Banner" src="https://baywolf-studios.github.io/jellyfin-plugin-repo/images/TvHeadend%20Client.png"/>
<br/>
<br/>

## About

This plugin securely connects Jellyfin with your TvHeadend setup. Stream live TV, navigate the full EPG, and control DVR scheduling. It exclusively uses Digest Authentication for secure access and proxies Tvheadend's image cache.

**Note**: This plugin exclusively uses the TvHeadend JSON API via Digest Authentication and does not support the custom HTSP protocol.

## Features

- **Live TV Support**
- **EPG Integration**
- **Recording Management**
- **Automatic Series Timers**
- **Digest Authentication**
- **Recordings Channel**
- **And more!**

## Requirements

- **Jellyfin 10.10 or later**
- **TvHeadend 4.3 or later**
- A configured TvHeadend server with accessible API endpoints.

## Installation

1. Open Jellyfin dashboard
2. Naviage to to Plugins -> Catalog settings
3. Add the repository
```
https://baywolf-studios.github.io/jellyfin-plugin-repo/manifest.json
```
4. Install the `TvHeadend Client` plugin from the repository.

## Usage

1. Ensure your TvHeadend instance is set up and running.
2. Fill in the URL to your TvHeadend instance in the plugin settings.
3. Provide additional configuration, such as:
    - **Username** and **Password** for TvHeadend API access.
4. Click `Save`
5. Refresh Guide
6. Navigate to the Jellyfin live TV section or EPG to access your channels and recordings.

**Note**: All configuration changes are applied at runtime without restarting the plugin.

## Configuration

The following settings are configurable within the plugin:

### Settings
- **Host**
- **Port**
- **Use SSL**
- **Username**
- **Password**
- **Hide Recordings Channel**
- **Force all Programs as Series**
  
**Note**:
- All requests use Digest MD5 Authentication via HTTP headers.

## Build

1. To build this plugin you will need [.Net 8.x](https://dotnet.microsoft.com/download/dotnet/8.0).

2. Build plugin with following command
  ```
  dotnet publish --configuration Release
  ```

## License

This project is licensed under the GNU General Public License v3.0 (GPL-3.0). See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please submit issues, feature requests, or pull requests to improve the plugin.

## Acknowledgments

- [Jellyfin](https://jellyfin.org)
- [TvHeadend](https://tvheadend.org)
- [jellyfin-plugin-tvheadend](https://github.com/jellyfin/jellyfin-plugin-tvheadend)
- [jellyfin-plugin-tvheadend-api](https://github.com/john-pierre/jellyfin-plugin-tvheadend-api)

---

### Contact
For questions or support, please open an issue or contact the repository maintainers.
