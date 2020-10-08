module SchemaTests

open Avro
open Avro.FSharp.Schema

open Expecto
open Expecto.Flip
open Foo.Bar

let expectSchemasEqual (actual:Schema) (expected:Schema) =
    Expect.equal "Schemas should be equal" (expected.ToString()) (actual.ToString())

[<Tests>]
let primitiveTests =
    testList "Primitive" [
        let pairs = [
            "string", typeof<string>
            "boolean", typeof<bool>
            "int", typeof<int>
            "long", typeof<int64>
            "float", typeof<float32>
            "double", typeof<float>
            "bytes", typeof<byte array>
        ]
        for typeName,type' in pairs ->
            test typeName {
                generateSchema type'
                |> Expect.wantOk "Schema should be created"
                |> expectSchemasEqual <| PrimitiveSchema.NewInstance(typeName)
            }
    ]
    |> testLabel "Schema"


[<Tests>]
let enumTests =
    testList "Enum" [
        test "TestState" {
            generateSchema typeof<TestState>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "enum", "name": "Foo.Bar.TestState", "symbols": ["Red", "Yellow", "Green"]}""")
        }
    ]|> testLabel "Schema"    

[<Tests>]
let arrayTests =
    testList "Array" [
        test "List" {
            generateSchema typeof<string list>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "array","items": "string"}""")
        }
        test "Array" {
            generateSchema typeof<int array>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "array","items": "int"}""")
        }
        test "Generic.List" {
            generateSchema typeof<System.Collections.Generic.List<bool>>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "array","items": "boolean"}""")
        }
    ]|> testLabel "Schema"    

[<Tests>]
let mapTests =
    testList "Map" [
        test "Map" {
            generateSchema typeof<Map<string,string>>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "map","values": "string"}""")
        }
        test "Generic.Dictionary" {
            generateSchema typeof<System.Collections.Generic.Dictionary<string, int>>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "map","values": "int"}""")
        }
    ]|> testLabel "Schema"    

[<Tests>]
let recordTests =
    testList "Record" [
        test "SimpleRecord" {
            generateSchema typeof<SimpleRecord>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type": "record",
                "name": "Foo.Bar.SimpleRecord",
                "fields" : [
                    {"name": "Id", "type": "int"},            
                    {"name": "Name", "type": "string"},
                    {"name": "Version", "type": "long"}
                ]                
            }""")
        }
        test "HierarchyRecord" {
            generateSchema typeof<HierarchyRecord>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type": "record",
                "name": "Foo.Bar.HierarchyRecord",
                "fields" : [
                    {"name": "Title", "type": "string"},            
                    {"name": "Details", "type": {
                        "type": "record",
                        "name": "Foo.Bar.SimpleRecord",
                        "fields" : [
                            {"name": "Id", "type": "int"},            
                            {"name": "Name", "type": "string"},
                            {"name": "Version", "type": "long"}
                        ]                
                    }},
                    {"name": "IsProcessed", "type": "boolean"}
                ]                
            }""")
        }
        test "Option" {
            generateSchema typeof<Option<float>>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""[
                    {"type":"record","name":"None","namespace":"Microsoft.FSharp.Core.FSharpOption_Of_System_Double","fields":[]},
                    {"type":"record","name":"Some","namespace":"Microsoft.FSharp.Core.FSharpOption_Of_System_Double","fields":[
                        {"name":"Value","type":"double"}
                    ]}
                ]""")
        }
        test "Result" {
            generateSchema typeof<Result<int64,string>>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""[
                    {"type":"record","name":"Ok","namespace":"Microsoft.FSharp.Core.FSharpResult_Of_System_Int64_And_System_String","fields":[
                        {"name":"ResultValue","type":"long"}
                    ]},
                    {"type":"record","name":"Error","namespace":"Microsoft.FSharp.Core.FSharpResult_Of_System_Int64_And_System_String","fields":[
                        {"name":"ErrorValue","type":"string"}]
                    }
                ]""")
        }
        test "BinaryTree" {
            generateSchema typeof<BinaryTree>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type":[
                    {
                        "type":"record",
                        "name":"Leaf",
                        "namespace":"Foo.Bar.BinaryTree",
                        "fields":[
                            {"name":"value","type":"string"}
                        ]
                    },
                    {
                        "type":"record",
                        "name":"Node",
                        "namespace":"Foo.Bar.BinaryTree",
                        "fields":[
                            {"name":"left","type":["Leaf","Node"]},
                            {"name":"right","type":["Leaf","Node"]}
                        ]
                    }
                ]}""")
        }
    ]|> testLabel "Schema"    

[<Tests>]
let tupleTests =
    testList "Tuple" [
        test "Tuple" {
            generateSchema typeof<int*string>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type": "record",
                "name": "System.Tuple_Of_System_Int32_And_System_String",
                "fields" : [
                    {"name": "Item1", "type": "int"},            
                    {"name": "Item2", "type": "string"},
                ]                
            }""")
        }
    ]|> testLabel "Schema"    

[<Tests>]
let logicalTypesTests =
    testList "LogicalTypes" [
        test "Decimal" {
            generateSchema typeof<decimal>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "bytes", "logicalType": "decimal", "precision": 29, "scale": 14}""")
        }
        test "UIID" {
            generateSchema typeof<System.Guid>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "string", "logicalType": "uuid"}""")
        }
        test "DateTime" {
            generateSchema typeof<System.DateTime>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "string"}""")
        }
        test "TimeSpan" {
            generateSchema typeof<System.TimeSpan>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "string"}""")
        }
        test "DateTimeOffset" {
            generateSchema typeof<System.DateTimeOffset>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "string"}""")
        }
        test "Uri" {
            generateSchema typeof<System.Uri>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{"type": "string"}""")
        }
    ]|> testLabel "Schema"    

[<Tests>]
let annotationsTests =
    testList "Annotations" [
        test "Aliases" {
            generateSchema typeof<NewRecord>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type": "record",
                "name": "Foo.Bar.NewRecord",
                "aliases": ["OldRecord"],
                "fields" : [
                    {"name": "Id", "type": "int", "default": 42},            
                    {"name": "Caption", "aliases": ["Title", "Cap"], "type": "string"},
                    {"name": "Price", "type": {"type": "bytes", "logicalType": "decimal", "precision": 29, "scale": 3}},
                ]                
            }""")
        }
        test "EnumAliasesAndDefaultValue" {
            generateSchema typeof<NewTestState>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type": "enum", 
                "name": "Foo.Bar.NewTestState", 
                "aliases":["Foo.Bar.TestState"], 
                "symbols": ["Red", "Yellow", "Green", "Blue"]
            }""")
        }
    ]|> testLabel "Schema"    


[<Tests>]
let complexTypesTests =
    testList "ComplexTypes" [
        test "ItemRecord" {
            generateSchema typeof<ItemRecord>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type":"record",
                "name":"Foo.Bar.ItemRecord",
                "fields":[
                    {"name":"Id","type":"int"},
                    {"name":"Name","type":"string"},
                    {"name":"Price","type":[
                        {
                            "type":"record",
                            "name":"Foo.Bar.Price.Price",
                            "fields":[
                                {"name":"Item","type":{"type":"bytes","logicalType":"decimal","precision":29,"scale":14}}
                            ]
                        }
                    ]}
                ]}""")
        }
        test "Basket" {
            generateSchema typeof<Basket>
            |> Expect.wantOk "Schema should be created"
            |> expectSchemasEqual <| Schema.Parse ("""{
                "type":"array",
                "items":[
                    {
                        "type":"record",
                        "name":"Foo.Bar.LineItem.SaleItem",
                        "fields":[
                            {"name":"Item1","type":{
                                "type":"record",
                                "name":"System.Tuple_Of_System_String_And_System_String_And_System_Decimal",
                                "fields":[
                                    {"name":"Item1","type":"string"},
                                    {"name":"Item2","type":"string"},
                                    {"name":"Item3","type":{"type":"bytes","logicalType":"decimal","precision":29,"scale":14}}
                                ]
                            }},
                            {"name":"Item2","type":{"type":"bytes","logicalType":"decimal","precision":29,"scale":14}}
                        ]
                    },
                    {
                        "type":"record",
                        "name":"Foo.Bar.LineItem.TenderItem",
                        "fields":[
                            {"name":"Item1","type":[
                                {
                                    "type":"record",
                                    "name":"Foo.Bar.Tender.Cash",
                                    "fields":[]
                                },
                                {
                                    "type":"record",
                                    "name":"Foo.Bar.Tender.Card",
                                    "fields":[
                                        {"name":"Item","type":"string"}
                                    
                                    ]
                                },
                                {
                                    "type":"record",
                                    "name":"Foo.Bar.Tender.Voucher",
                                    "fields":[{"name":"Item","type":"string"}]
                                }]
                            },
                            {"name":"Item2","type":
                                {"type":"bytes","logicalType":"decimal","precision":29,"scale":14}
                            }
                        ]
                    },
                    {
                        "type":"record",
                        "name":"Foo.Bar.LineItem.CancelItem",
                        "fields":[{"name":"Item","type":"int"}]
                    }
                ]}""")

        }
    ]|> testLabel "Schema"