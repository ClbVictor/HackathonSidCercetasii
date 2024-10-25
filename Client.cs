{
  "openapi": "3.0.0",
  "info": {
    "title": "OpenAI API",
    "version": "1.0.0"
  },
  "paths": {
    "/v1/completions": {
      "post": {
        "summary": "Create a completion",
        "operationId": "completions",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "type": "object",
                "properties": {
                  "model": {
                    "type": "string"
                  },
                  "prompt": {
                    "type": "string"
                  },
                  "max_tokens": {
                    "type": "integer"
                  },
                  "temperature": {
                    "type": "number"
                  }
                }
              }
            }
          },
          "required": true
        },
        "responses": {
          "200": {
            "description": "Successful response",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "choices": {
                      "type": "array",
                      "items": {
                        "type": "object",
                        "properties": {
                          "text": {
                            "type": "string"
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    },
    "/v1/images": {
      "post": {
        "summary": "Generate an image based on input",
        "operationId": "images",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "type": "object",
                "properties": {
                  "prompt": {
                    "type": "string"
                  },
                  "n": {
                    "type": "integer"
                  },
                  "size": {
                    "type": "string"
                  }
                }
              }
            }
          },
          "required": true
        },
        "responses": {
          "200": {
            "description": "Successful response",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "data": {
                      "type": "array",
                      "items": {
                        "type": "object",
                        "properties": {
                          "url": {
                            "type": "string"
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  },
  "components": {}
}