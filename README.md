## Overview
This solution is a .NET 8-based API designed to handle currency exchange operations. It provides a structured and efficient way to convert amounts between different currencies using a common API structure.

## Features
Currency Conversion: Get the exchange rate from 3rd party API and Convert amounts from one currency to another.

## Installation
git clone 

## Config
ExchangeRateConfig: you should set 3rd party information in the config file (appSettings.json) in Assessment.API. You can change CacheTime in config to optimise the round trip to 3rd party.

## API Call
Sample: 
curl -X 'POST' \
  'http://localhost:50000/api/ExchangeService' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "amount": 100,
  "inputCurrency": "AUD",
  "outputCurrency": "USD"
}'

## Additional documentation
To change the 3rd Party only add a new implementation of IExchangeRateService and registerd it to DI