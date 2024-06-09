module DogApi

open Fable.SimpleHttp
open Thoth.Json

type DogImage = {
    message: string
    status: string
} with

    static member Decoder =
        Decode.object (fun get -> {
            message = get.Required.Field "message" Decode.string
            status = get.Required.Field "status" Decode.string
        })

[<Literal>]
let url = "https://dog.ceo/api/breeds/image/random"

let fetchDog () = async {
    let! (code, data) = Http.get url
   
    if code = 200 then
        let image = Decode.fromString DogImage.Decoder data
        return image
    else
        return Error (sprintf "reponse code: %d" code)
}