# OhHeck
Tool to validate and optimize Chroma and Noodle maps


# OhHeck.CLI
- Build OhHeck.CLI project using `dotnet build`
- Example invocation: `./OhHeck.CLI --suppressed-warnings string-bool;ends-with-regex-lookup;similar-point-data-slopee --configure similar-point-data-slope:difference_threshold=5`
    - `--suppressed-warnings {warning1; warning2;}` (seperated by `;`) Disables warnings listed by their name
    - `--configure {warning_name:property_name=value}` Configures the property of the warning to the value. Will silently ignore any invalid/unknown warnings/properties
    - `--warning-count {count}` Limits the maximum amount of warnings displayed
    - `--map path/to/map` Specifies the directory of the map to analyze