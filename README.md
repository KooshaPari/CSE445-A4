# CSE445 Assignment 4 - XML Processing

This repository contains the XML files and C# implementation for CSE445 Assignment 4.

## Files

- `Hotels.xml` - Valid XML file containing hotel information
- `Hotels.xsd` - XML Schema Definition for hotel structure
- `HotelsErrors.xml` - XML file with intentional errors for testing
- `submission.cs` - C# implementation with validation and JSON conversion methods

## Live URLs

- Hotels.xml: https://raw.githubusercontent.com/KooshaPari/CSE445-A4/main/Hotels.xml
- Hotels.xsd: https://raw.githubusercontent.com/KooshaPari/CSE445-A4/main/Hotels.xsd  
- HotelsErrors.xml: https://raw.githubusercontent.com/KooshaPari/CSE445-A4/main/HotelsErrors.xml

## Assignment Requirements

1. XML Schema (Hotels.xsd) - Defines structure for hotel directory
2. Valid XML (Hotels.xml) - Contains 10+ real hotels following the schema
3. Error XML (HotelsErrors.xml) - Contains 5 specific validation errors
4. C# Methods:
   - Verification() - Validates XML against XSD
   - Xml2Json() - Converts XML to JSON format
   - Main() - Tests both methods

## Features

- XML validation with detailed error reporting
- XML to JSON conversion with proper structure
- Support for both local files and remote URLs
- Handles optional attributes correctly
- Compatible with Newtonsoft.Json deserialization
