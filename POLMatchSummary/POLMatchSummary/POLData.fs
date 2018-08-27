namespace POL

open System
open System.IO
open System.Configuration
open System.Web
open System.Data
open System.Collections.Generic
open SeasideResearch.LibCurlNet
open Newtonsoft.Json


module POLData =

    
    ///Team Summary
    type TeamSummary =
        {
            _5m: int
            _6m: int
            _7m: int
            _8m: int
            _9m: int
            bed: int
            comb: string
            combinado: string
            curPage: int
            eye: int
            hat: int
            highton: int
            htoff: int
            imgMd5Txt: string
            lowton: int
            ltoff: int
            matchDraw: int
            matchLose: int
            matchRatio: int
            matchWin: int
            mobilePage: int
            mpr: string
            pageCnt: int
            penaltyPt: int
            ppd: string
            rankNum: int
            rndNum: int
            rnkgDcsnTpCd: string
            rtg: string
            scrollHeight: int
            searchStageRndNum: int
            seedNo: int
            setDraw: int
            setLose: int
            setPt: int
            setRatio: int
            setTotal: int
            setWin: int
            shpId: string
            shpNm: string
            sum_5m: int
            sum_6m: int
            sum_7m: int
            sum_8m: int
            sum_9m: int
            sum_bed: int
            sum_eye: int
            sum_hat: int
            sum_highton: int
            sum_htoff: int
            sum_lowton: int
            sum_ltoff: int
            sum_ton80: int
            sum_wh: int
            teamId: string
            teamNm: string
            ton80: int
            totalCount: int
            totalPt: int
            wh: int
            winPt: int
        }


    ///Team Detail
    type TeamDetail =
        {
            brcktId: string
            cptNm: string
            cpttnId: string
            curPage: int
            fetchSize: int
            imgMd5Txt: string
            matchDraw: int
            matchLose: int
            matchRatio: string
            matchTotal: int
            matchWin: int
            pageCnt: int
            penaltyPt: int
            placeNm: string
            rankNum: int
            rankPtNum: int
            reqPage: int
            resultPageInfo: int
            rndNum: int
            setDraw: int
            setLose: int
            setPtNum: int
            setRatio: string
            setTotal: int
            setWin: int
            shpId: string
            shpNm: string
            teamComb: string
            teamCombinado: string
            teamId: string
            teamMpr: string
            teamNm: string
            teamPpd: string
            teamRtg: string
            topN: int
            totalPt: int
            winPtNum: int
        }

    ///Team Detail Page
    type TeamDetailPage =
        {
            teamDetail: TeamDetail
        }


    ///Team Match History
    type TeamMatchHistory =
        {
            brcktId: string
            cpttnId: string
            cpttnNm: string
            curPage: int
            mtchSttCd: int
            pageCnt: int
            plyngStrtDt: string
            plyngStrtDtM: string
            shpAId: string
            shpANm: string
            shpBId: string
            shpBNm: string
            stageId: string
            teamAId: string
            teamANm: string
            teamARsltPt: string
            teamAWinFg: string
            teamBId: string
            teamBNm: string
            teamBRsltPt: string
            teamBWinFg : string
        }

    ///Team History Page
    type TeamMatchHistoryPage =
        {
            matchTeamHistCnt: int
            matchTeamHistList: TeamMatchHistory[]
        }



    ///Player Result/Detail
    type TeamPlayerResult =
        {
            _5m: int
            _6m: int
            _7m: int
            _8m: int
            _9m: int
            areaCnt: int
            bed: int
            cSeq: string
            comb: string
            combinado: string
            cpttnCls: string
            cpttnId: string
            curPage: int
            eye: int
            hat: int
            highton: int
            htoff: int
            lowton: int
            ltoff: int
            matchLose: int
            matchWin: int
            mpr: string
            pageCnt: int
            penaltyPt: int
            plyrId: string
            plyrNm: string
            ppd: string
            rankNum: int
            resultPageInfo: int
            rtg: string
            setDraw: int
            setLose: int
            setPt: int
            setRatio: int
            setTotal: int
            setWin: int
            shpId: string
            shpNm: string
            sum_5m: int
            sum_6m: int
            sum_7m: int
            sum_8m: int
            sum_9m: int
            sum_bed: int
            sum_eye: int
            sum_hat: int
            sum_highton: int
            sum_htoff: int
            sum_lowton: int
            sum_ltoff: int
            sum_ton80: int
            sum_wh: int
            teamId: string
            teamNm: string
            ton80: int
            totalCount: int
            totalPt: int
            userSeq: int
            wh: int
            winPt: int
        }

    ///Player Result Page
    type TeamPlayerResultPage =
        {
            plyrResultCnt: int
            plyrResultList: TeamPlayerResult[]
        }



    ///Team Detail
    type ShopDetail =
        {
            shopId: string
            latitude: double
            sSeq: int
            nationcode: string
            longitude: double
            phoneNo: string
            name: string
            address: string
        }

    ///Team Detail Page
    type ShopDetailPage =
        {
            teamDetail: ShopDetail
        }


    let NullShop =
        {name="Invalid Shop"; shopId=""; latitude=0.0; sSeq=0; nationcode=""; longitude=0.0; phoneNo=""; address=""}


    let mutable buffer:Byte[] = [||]

    let debugMode = 
                    try
                        (ConfigurationManager.AppSettings.["Verbose"].ToUpper().Equals("YES") || ConfigurationManager.AppSettings.["Verbose"].ToUpper().Equals("TRUE"))
                    with
                        | exn -> false



    ///LibCurl helpter function
    let OnWriteData (buf:Byte[]) (size:Int32) (nmemb:Int32) (extraData:Object) = 
        Console.Write(System.Text.Encoding.UTF8.GetString(buf))
        size * nmemb

    ///LibCurl helpter function
    let OnDebug (infoType:CURLINFOTYPE) (msg:String) (extraData:Object) = 
        Console.WriteLine("Curl Debug: {0}", msg)


    ///LibCurl helpter function
    let WriteToBuffer (buf:Byte[]) (size:Int32) (nmemb:Int32) (extraData:Object) = 
        buffer <- Array.append buffer buf
        size * nmemb
    

    ///Use lubCurl to read data from Reset API
    let readRestData(uri:string, writeFunction, verbose:bool) = 
  
        try
        
            Console.WriteLine( "Connecting to : {0}\n", uri)

            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL) |> ignore

            //Easy Reader
            let easy = new Easy()

            let wf = new Easy.WriteFunction(writeFunction)
            let df = new Easy.DebugFunction(OnDebug)


            //if verbose then easy.SetOpt(CURLoption.CURLOPT_VERBOSE, 1) |> ignore

            easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYHOST, 0) |> ignore
            easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, 0) |> ignore
            easy.SetOpt(CURLoption.CURLOPT_URL, uri) |> ignore

            easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf) |> ignore
            easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df) |> ignore
            
            buffer <- [||]
            
            let easyResult = easy.Perform()
             
            easy.Dispose() |> ignore
            Curl.GlobalCleanup() |> ignore
        
            easyResult
        
        with 
            | exn -> 
                Console.WriteLine( "Error downloading data from REST API : {0} {1}", exn.Message, exn.StackTrace ) |> ignore
                CURLcode.CURLE_FAILED_INIT



    ///Deserialize JSON to specified object type
    let DeserializeJSON(json:byte[], T:Type) = 

        let mutable objects:'T[] = [||]
    
        let jsonText = System.Text.Encoding.UTF8.GetString(json)
        
        //Remove start and end brackets
        let jsonText = if (jsonText.StartsWith("[") && jsonText.EndsWith("]")) then jsonText.Remove(jsonText.Length-1).Remove(0,1) else jsonText

        let reader = new JsonTextReader(new StringReader(jsonText))
        let serializer = new JsonSerializer()
     
        reader.SupportMultipleContent <- true
     
        while reader.Read() do
            let obj = serializer.Deserialize<'T>(reader)
            objects <- Array.append objects [|obj|]

        objects    

        

    ///Reads text from file
    let readFromTextFile(filePath:string) = 
        seq {
            use sr = new StreamReader (filePath)
            while not sr.EndOfStream do
                yield sr.ReadLine ()
        }
        |> String.concat("\n")




    ///Read teams in group from POL
    let readTeams(competitionId:String, divisionId:string, stageId:string, teamId:string) = 
        
        let uri = String.Format("http://play.phoenixdart.com/selectTeamRankingAwardListMLJson.do?cpttnId={0}&searchDivision={1}&searchStage={2}&searchTeam={3}", competitionId, divisionId, stageId, teamId)

        //Read from API
        buffer <- [||]
        let curlResult = readRestData(uri, WriteToBuffer, true)

        //Deserialize data
        let teams = 
            if (curlResult=CURLcode.CURLE_OK) then //If response OK
                let teams:TeamSummary[] = DeserializeJSON(buffer, typeof<TeamSummary>)
                if ((teams<>null) && (teams.Length>0)) then
                    teams
                else
                    [||]
            else
                [||]
        teams 



    ///Read team details from POL
    let readTeamDetail(competitionId:String, divisionId:string, stageId:string, teamId:string) = 
        
        let uri = String.Format("http://play.phoenixdart.com/selectTeamRankingInfoMLJson.do?cpttnId={0}&searchDivision={1}&searchStage={2}&searchTeam={3}", competitionId, divisionId, stageId, teamId)

        //Read from API
        buffer <- [||]
        let curlResult = readRestData(uri, WriteToBuffer, true)

        //Deserialize data
        let teamDetail =
            if (curlResult=CURLcode.CURLE_OK) then //If response OK
                let teamDetailPages:TeamDetailPage[] = DeserializeJSON(buffer, typeof<TeamDetailPage>)
                if ((teamDetailPages<>null) && (teamDetailPages.Length>0)) then
                    [|teamDetailPages.[0].teamDetail|]
                else
                    [||]
            else
                [||]
        
        teamDetail 



    ///Read team match history from POL
    let readTeamMatchHistory(competitionId:String, divisionId:string, stageId:string, teamId:string) = 
        
        let mutable hasMoreData = true
        let mutable curPage = 1
        let mutable teamMatchHistories:TeamMatchHistory[] = [||]
        
        while ((hasMoreData) && (curPage<=10)) do

            let uri = String.Format("http://play.phoenixdart.com/selectMatchTeamHistListMLJson.do?cpttnId={0}&searchDivision={1}&searchStage={2}&searchTeam={3}&curPage={4}", competitionId, divisionId, stageId, teamId, curPage)

            //Read from API
            buffer <- [||]
            let curlResult = readRestData(uri, WriteToBuffer, true)

            //Deserialize data
            if (curlResult=CURLcode.CURLE_OK) then //If response OK
                let teamMatchHistoryPages:TeamMatchHistoryPage[] = DeserializeJSON(buffer, typeof<TeamMatchHistoryPage>)
                if ((teamMatchHistoryPages<>null) && (teamMatchHistoryPages.Length>0)) then
                    let matchHistoriesCurPage = teamMatchHistoryPages.[0].matchTeamHistList
                    if ((matchHistoriesCurPage<>null) && (matchHistoriesCurPage.Length>0)) then
                        teamMatchHistories <- Array.append teamMatchHistories matchHistoriesCurPage
                        curPage <- curPage + 1
                    else
                        hasMoreData <- false
                else
                    hasMoreData <- false
            else
                hasMoreData <- false
        
        teamMatchHistories 




    ///Read team player details and results from POL
    let readTeamPlayerResult(competitionId:String, divisionId:string, stageId:string, teamId:string) = 
        
        let uri = String.Format("http://play.phoenixdart.com/selectPlyrResultAwardListWithCntMLJson.do?cpttnId={0}&searchDivision={1}&searchStage={2}&searchTeam={3}", competitionId, divisionId, stageId, teamId)

        //Read from API
        buffer <- [||]
        let curlResult = readRestData(uri, WriteToBuffer, true)

        //Deserialize data
        let teamPlayerResults = 
            if (curlResult=CURLcode.CURLE_OK) then //If response OK
                let teamPlayerResultPages:TeamPlayerResultPage[] = DeserializeJSON(buffer, typeof<TeamPlayerResultPage>)
                if ((teamPlayerResultPages<>null) && (teamPlayerResultPages.Length>0)) then
                    let playerResultsCurPage = teamPlayerResultPages.[0].plyrResultList
                    if ((playerResultsCurPage<>null) && (playerResultsCurPage.Length>0)) then
                        playerResultsCurPage
                    else
                        [||]
                else
                    [||]
            else
                [||]
        
        teamPlayerResults 



    ///Read shop details from POL
    let readShopDetail(shopId:string) = 
        
        let uri = String.Format("http://play.phoenixdart.com/selectShpAreaListDetailJson.do?searchShp={0}", shopId)

        //Read from API
        buffer <- [||]
        let curlResult = readRestData(uri, WriteToBuffer, true)

        //Deserialize data
        let shopDetail =
            if (curlResult=CURLcode.CURLE_OK) then //If response OK
                let shopDetail:ShopDetail[] = DeserializeJSON(buffer, typeof<ShopDetail>)
                if ((shopDetail<>null) && (shopDetail.Length>0)) then
                    shopDetail
                else
                    [||]
            else
                [||]
        
        shopDetail 



    ///find next match in array of matches
    let findNextMatch(matches:TeamMatchHistory[]) = 
        
        matches
            |> Array.filter(fun m -> DateTime.Parse(m.plyngStrtDt.Substring(0, 10)) >= DateTime.Today)
            |> Array.minBy(fun m -> DateTime.Parse(m.plyngStrtDt.Substring(0, 10)))





