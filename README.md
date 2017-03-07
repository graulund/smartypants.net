# SmartyPants.Net

A simplified .NET port of the typography prettifier framework SmartyPants.

Note however, that **this library only works with plain text strings**, and is seen as a solution that's independent of the medium — so it does not assume that HTML entities or tags are possible.

Strings like this:

> He said, "'Quoted' words in a larger quote" -- or did he...?

Will be converted to:

> He said, “‘Quoted’ words in a larger quote” — or did he…?

## Usage

Create a  `SmartyPants` object with your preferred settings, and call the `Educate` method to get an educated string:

```C#
// Default setting
new SmartyPants().Educate("Now -- and then");

// Non-default setting
new SmartyPants(SmartyPants.ATTR_SHORT_EM_DASH_LONG_EN).Educate("Now -- and then");
```

### Settings

The following settings are available, ported from the original version of SmartyPants, which can be sent as an argument to the constructor:

* `SmartyPants.ATTR_DO_NOTHING`: No transformations are applied.
* `SmartyPants.ATTR_EM_DASH`: Use `--` for em dashes; no en dash support. (Default)
* `SmartyPants.ATTR_LONG_EM_DASH_SHORT_EN`: Use `---` for em dashes, `--` for en dashes.
* `SmartyPants.ATTR_SHORT_EM_DASH_LONG_EN`: Use `--` for em dashes, `---` for en dashes.
* `SmartyPants.ATTR_STUPEFY`: Stupefy the string; that is, perform the reverse operation.

In addition, the constructor can be sent a string with custom flags that changes its behavior. The flags are as follows:

* `q`: quotes
* `b`: backtick quotes (\`\`double'' only)
* `B`: backtick quotes (\`\`double'' and \`single')
* `d`: dashes
* `D`: old school dashes (same as `ATTR_LONG_EM_DASH_SHORT_EN` above)
* `i`: inverted old school dashes (same as `ATTR_SHORT_EM_DASH_LONG_EN` above)
* `e`: ellipses

For example, you could send the string `"bDe"` to only convert double backticks, convert dashes according to the "old school dashes" setting, and convert ellipses.

## Authors

The port is created by Andy Graulund, based on Michel Fortin's [PHP port](https://github.com/michelf/php-smartypants), which is in turn based on the original [SmartyPants by John Gruber](https://daringfireball.net/projects/smartypants/).
