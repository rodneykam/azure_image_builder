# azure_image_builder

This is the home page of the MyPackage source code repositories and is intended for those contributing to MyPackage or using bleeding edge nightly builds.

MyPackage allows you to...

## Get Started

## Downloading the azure_image_builder

In order to use azure_image_builder, you'll need to download the [JFrog CLI](https://jfrog.com/getcli/). Its recommended to put the tool in your PATH, so that you can use it from terminal.

You'll need to decide on a version to download. In a browser, you can navigate to the [Tools Repository](https://packages.relayhealth.com/webapp/#/artifacts/browse/tree/General/tools-release-custom-local/azure_image_builder) and view the versions. 

Assuming you want to download version `1.2.3.4` to a `azure_image_builder` directory you'll run the following command.

```
jfrog rt dl tools-release-custom-local/azure_image_builder/1.2.3.4/netstandard2.0/ azure_image_builder/
```

When you have .NET Core 2.1 runtime installed, you can use `netstandard2.0` as the platform. If you do not have .NET Core installed, then you can use one of the self contained packages.

### Running the azure_image_builder

If you have .NET Core 2.1 installed and you have downloaded just the binaries for `netstandard2.0`, then you can invoke azure_image_builder as follows:

```
dotnet azure_image_builder.dll help
```

If you downloaded the self contained package Linux or Mac, you'll need to mark the `azure_image_builder` as executable before you can run it.

```
chmod +x azure_image_builder
```

If you downloaded the self contained package for wither Windows, Linux, or Mac, you can execute azure_image_builder as follows:

```
azure_image_builder help
```

See the [documentation](docs/index.md) for more examples on how to use azure_image_builder.

## How to Contribute

### Building

To build the template on Windows, open a PowerShell session and run the following:

```
.\build.ps1
```

On a Unix like operation system like macOS or Linux, you can use bash:

```
./build.sh
```

This will produce the binaries in the `target` directory, one for each supported platform.

### Testing

azure_image_builder comes with functional tests. This allows developers to test functionality in isolation before committing code to the repository.  

To provision systems, perform the following steps:

1. Do A
2. Do B
3. Do C

To deploy the systems, perform the following steps:

1. Do A
2. Do B
3. Do C

In order to run the tests against the system

Create the following file in `%APPDATA%\chc\MyPackage\testing\appsettings.json`. If you're using a Unix like operating system, create the file at "~/chc/MyPackage/testing/appsettings.json"
```
{
    "Environment": {
        "Credentials": {
            "ClientId": "123",
            "ClientSecret": "465",
            "TenantId": "678"
        },
        "Prefix": "ZZDEV",
        "Subscription: "9123"
    }
}
```

You can then run the tests using Visual Studio or on command line.