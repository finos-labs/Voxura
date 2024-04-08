![badge-labs](https://user-images.githubusercontent.com/327285/230928932-7c75f8ed-e57b-41db-9fb7-a292a13a1e58.svg)

# Voxura

Voxura focuses on gaining real-time insights and extracting data from speech.

Here is the original (Speech-to-Form) example of what Voxura can do:
[![image](https://github.com/finos-labs/Voxura/assets/1344888/866caf48-55ca-4a04-b47f-842bf4ba57be)](https://www.youtube.com/watch?v=CiGu_okP-c8)

(You can see the code for this in the 'First POC' folder in this repo.)

Here is a screenshot of the WPF demo:

![image](https://github.com/finos-labs/Voxura/assets/1344888/37ae7eb9-26bc-42a6-8273-4af255701727)

## Roadmap

1. (75% done) Voxura.Core - the core functionality of Voxura, for embeddig scenarios:
	a. configuration of LLMs and prompts
	b. processing speech input
	c. extracting data from speech
	d. returning JSON data
2. (75% done) Voxura.MauiDemo - a demo app for Voxura.Core, using .NET MAUI. Demos for Android, Windows, Linux
3. (75% done) Voxura.WpfDemo - a demo app for Voxura.Core, using WPF (Windows only)
4. (0% done) Voxura.WebAPI - a web api wrapper around Voxura.Core, with the additional features:
    a. configuration of LLMs and prompts
	b. access control
	c. streaming speech or text input
5. (0% done) Voxura.WebDemo - a web demo using Voxura.WebAPI
6. (0% done) Voxura.BlazorDemo - a Blazor demo using Voxura.WebAPI


## Installation

- No releases yet, just clone the repo

## Development setup

- Clone the repo
- Enjoy


## Using DCO to sign your commits

All commits must be signed with a DCO signature to avoid being flagged by the DCO Bot. This means that your commit log message must contain a line that looks like the following one, with your actual name and email address:

```
Signed-off-by: John Doe <john.doe@example.com>
```

Adding the `-s` flag to your `git commit` will add that line automatically. You can also add it manually as part of your commit log message or add it afterwards with `git commit --amend -s`.

### Helpful DCO Resources
- [Git Tools - Signing Your Work](https://git-scm.com/book/en/v2/Git-Tools-Signing-Your-Work)
- [Signing commits
](https://docs.github.com/en/github/authenticating-to-github/signing-commits)

## Contributing

1. Fork it (<https://github.com/finos/Voxura/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Read our [contribution guidelines](.github/CONTRIBUTING.md) and [Community Code of Conduct](https://www.finos.org/code-of-conduct)
4. Commit your changes (`git commit -am 'Add some fooBar'`)
5. Push to the branch (`git push origin feature/fooBar`)
6. Create a new Pull Request

## License

Copyright 2024 FINOS

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
