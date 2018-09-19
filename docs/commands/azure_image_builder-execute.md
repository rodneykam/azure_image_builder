# azure_image_builder Execute

## Name

`azure_image_builder execute` - Searches for snowflakes in the sun

## Synopsis

```
azure_image_builder execute [-l|--verbosity] [-v|--vault] [-c|--client-id] [-p|--client-secret]
```

## Description

The `azure_image_builder execute` command searches the void for snowflakes in the sun. The result of this command are some snowflakes. If a client id, client secret and vault was specified, then the settings will be read from the configuration.

## Arguments

This command has no arguments

## Options

`-l|--verbosity {Trace|Debug|Information|Warning|Error|Critical|None}`

Defines the verbosity of the azure_image_builder outputs. The default value is Information.

`-v|--vault`

The name of the vault where settings are stored.

`-c|--client-id`

The client id used to access the vault.

`-d|--client-secret`

The client secret used to access the vault.

## Examples

Searches for snowflakes in the sun:

```
azure_image_builder execute
```

Searches for snowflakes in the sun, printing debug messages

```
azure_image_builder execute --verbosity debug
```
