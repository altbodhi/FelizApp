module Contexts

open Feliz

type Session = { value: int; update: int -> unit }
let defaultValue = { value = 0; update = fun _ -> () }
let sessionContext = React.createContext (name = "Session", defaultValue = defaultValue)