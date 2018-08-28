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

    ///Player Result/Detail
    type TeamPlayerGeneral =
        {
            plyrId: string
            sexTpNm: string
        }

    ///Player detail
    type TeamPlayerSummary =
        {
            cSeq: string
            gender: string 
            mpr: string
            plyrId: string
            plyrNm: string
            ppd: string
            rankNum: int
            rtg: string
            setDraw: int
            setLose: int
            setRatio: int
            setTotal: int
            setWin: int
            teamId: string
            teamNm: string
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


    type MatchLineUp =
        {
            complete: bool
            set1: TeamPlayerSummary[]
            set2: TeamPlayerSummary[]
            set3: TeamPlayerSummary[]
            set4: TeamPlayerSummary[]
            set5: TeamPlayerSummary[]
            set6: TeamPlayerSummary[]
            set7: TeamPlayerSummary[]
            set8: TeamPlayerSummary[]
            set9: TeamPlayerSummary[]
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
    let readTeamPlayers(competitionId:String, divisionId:string, stageId:string, teamId:string) = 
        
        let uriResults = String.Format("http://play.phoenixdart.com/selectPlyrResultAwardListWithCntMLJson.do?cpttnId={0}&searchDivision={1}&searchStage={2}&searchTeam={3}", competitionId, divisionId, stageId, teamId)
        let uriPlayers = String.Format("http://play.phoenixdart.com/selectTeamMembersListJson.do?teamId={0}", teamId)

        //Read from API
        buffer <- [||]
        let curlResult = readRestData(uriResults, WriteToBuffer, true)

        //Deserialize data
        let teamPlayerResults = 
            if (curlResult=CURLcode.CURLE_OK) then //If response OK
                let teamPlayerResultPages:TeamPlayerResultPage[] = DeserializeJSON(buffer, typeof<TeamPlayerResultPage>)
                if ((teamPlayerResultPages<>null) && (teamPlayerResultPages.Length>0)) then
                    let playerResultsCurPage = teamPlayerResultPages.[0].plyrResultList
                    if ((playerResultsCurPage<>null) && (playerResultsCurPage.Length>0)) then
                        playerResultsCurPage
                        |> Array.map(fun p -> (p.plyrId, p))
                        |> dict

                    else
                        [||]
                        |> dict
                else
                    [||]
                    |> dict
            else
                [||]
                |> dict

        
        //Read from API
        buffer <- [||]
        let curlResult = readRestData(uriPlayers, WriteToBuffer, true)

        //Deserialize data
        let teamPlayers = 
            if (curlResult=CURLcode.CURLE_OK) then //If response OK
                let players:TeamPlayerGeneral[] = DeserializeJSON(buffer, typeof<TeamPlayerGeneral>)
                if ((players<>null) && (players.Length>0)) then
                    players
                        |> Array.map(fun p -> (p.plyrId, p.sexTpNm))
                        |> dict
                else
                    [||]
                    |> dict
            else
                [||]
                |> dict

        
        let teamPlayerIds = 
            teamPlayerResults.Values
                |> Seq.toArray
                |> Array.map(fun p -> p.plyrId)

        
        let teamPlayerSummaries = 
            [| for id in teamPlayerIds do
                    let succ, gen = teamPlayers.TryGetValue(id)
                    let gender = if succ then
                                    if gen.StartsWith("F") then "F" else "M"
                                 else
                                    "M"
                    let tpr = teamPlayerResults.Item(id)

                    yield
                        {plyrId=id; 
                        cSeq=tpr.cSeq;
                        gender=gender; 
                        mpr=tpr.mpr;
                        plyrNm=tpr.plyrNm;
                        ppd=tpr.ppd;
                        rankNum=tpr.rankNum;
                        rtg=tpr.rtg;
                        setDraw=tpr.setDraw;
                        setLose=tpr.setLose;
                        setRatio=tpr.setRatio;
                        setTotal=tpr.setTotal;
                        setWin=tpr.setWin;
                        teamId=tpr.teamId;
                        teamNm=tpr.teamNm;
                        }
                |]
        
        teamPlayerSummaries


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



    ///find strongest POL line-up for team
    let findStrongestLineUp(players:TeamPlayerSummary[]) = 
        
        let playersSorted = 
                        players
                            |> Array.sortBy(fun p -> Convert.ToDouble(p.rtg))

        let femalePlayers = 
                        playersSorted
                            |> Array.filter(fun p -> p.gender="F")

        
        if ((playersSorted.Length>=5) && (femalePlayers.Length>=1)) then
            
            let topFemalePlayer = femalePlayers.[0]
            let topPlayers = 
                    playersSorted
                        |> Array.filter(fun p -> p.plyrId <> topFemalePlayer.plyrId)
                        |> Array.take 5

            //Lineup Object
            {
                complete = true;
                set1= [|topPlayers.[0];topPlayers.[1];topPlayers.[4];topFemalePlayer|];
                set2= [|topPlayers.[0];topFemalePlayer|];
                set3= [|topPlayers.[1];topPlayers.[2]|];
                set4= [|topPlayers.[0];topPlayers.[3];topPlayers.[4];topFemalePlayer|];
                set5= [|topFemalePlayer|];
                set6= [|topPlayers.[3]|];
                set7= [|topPlayers.[2]|];
                set8= [|topPlayers.[1]|];
                set9= [|topPlayers.[0]|]
            }
        else
            {
                complete = true;
                set1= [||];
                set2= [||];
                set3= [||];
                set4= [||];
                set5= [||];
                set6= [||];
                set7= [||];
                set8= [||];
                set9= [||]
            }




