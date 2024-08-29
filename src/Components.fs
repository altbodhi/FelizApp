namespace App

open Feliz
open Feliz.Router

type Components =
    /// <summary>
    /// The simplest possible React component.
    /// Shows a header with the text Hello World
    /// </summary>
    [<ReactComponent>]
    static member HelloWorld() =
        let time = System.DateTime.UtcNow.ToString()
        Html.h1 time

    /// <summary>
    /// A stateful React component that maintains a counter
    /// </summary>
    [<ReactComponent>]
    static member Counter() =
        let store = React.useContext (Contexts.sessionContext)
        // let count, setCount = React.useState (store)

        Html.div
            [ Html.h1 store.value
              Html.button
                  [ prop.onClick (fun _ -> store.update (store.value + 1))
                    prop.text "Increment" ] ]

    [<ReactComponent>]
    static member DogShow() =
        let (error, image), setData = React.useState (("woof...", ""))
        let (isUpdate, update) = React.useState (false)

        let loadData () =
            async {
                try
                    let! record = DogApi.fetchDog ()
                    match record with
                    | Ok im -> setData ("", im.message)
                    | Error e -> setData (e, image)
                with exn ->
                    setData (sprintf "%s" exn.Message, image)
            }

        React.useEffect (loadData >> Async.StartImmediate, [| box isUpdate |])

        Html.div
            [ prop.children
                  [ if error.Length > 0 then
                        Html.span [ prop.className "error"; prop.text error ]
                    else
                        Html.img [ prop.className "dog-image"; prop.src image ]
                    Html.br []    
                    Html.button [ prop.onClick (fun _ -> update (not isUpdate)); prop.text "Update" ] ] ]


    [<ReactComponent>]
    static member MainMenu(location: list<string>) =
        let url = location |> List.tryHead |> Option.defaultValue ""

        Html.nav
            [ prop.className "main-menu"
              prop.children
                  [ Html.a
                        [ prop.className
                              [ if url = "" then
                                    "selected-menu" ]
                          prop.href (Router.format ("/"))
                          prop.text "Index" ]
                    Html.a
                        [ prop.className
                              [ if url = "counter" then
                                    "selected-menu" ]
                          prop.href (Router.format ("counter"))
                          prop.text "Counter" ]
                    Html.a
                        [ prop.className
                              [ if url = "hello" then
                                    "selected-menu" ]
                          prop.href (Router.format ("hello"))
                          prop.text "Hello" ] ] ]

    /// <summary>
    /// A React component that uses Feliz.Router
    /// to determine what to show based on the current URL
    /// </summary>
    [<ReactComponent>]
    static member Router() =
        let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())
        let (count, setCount) = React.useState (0)

        let router =
            React.router
                [ router.onUrlChanged updateUrl
                  router.children
                      [ Components.MainMenu(currentUrl)
                        match currentUrl with
                        | [] -> Components.DogShow()
                        | [ "hello" ] -> Components.HelloWorld()
                        | [ "counter" ] -> Components.Counter()
                        | otherwise -> Html.h1 "Not found" ] ]

        React.contextProvider (
            Contexts.sessionContext,
            { Contexts.Session.value = count
              Contexts.Session.update = setCount },
            React.fragment [ router ]
        )
