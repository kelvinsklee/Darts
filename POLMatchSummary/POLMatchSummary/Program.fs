open System
open System.IO
open System.Configuration
open System.Web
open System.Net
open System.Data
open System.Net.Mail
open System.Collections.Generic
open SeasideResearch.LibCurlNet
open Newtonsoft.Json
open POL.POLData



/// Send out SMTP email
let SendSMTPMail (mailFrom:MailAddress) (mailTo:string) (subject:string) (body:string) (attachments:string[])= 
    let mailMsg = new MailMessage()
    mailMsg.From <- mailFrom
    mailTo.Split [|','; ';'; ' '|]
    |> Array.filter(fun email -> email <> "")
    |> Array.iter(fun addr ->mailMsg.To.Add(addr))
    mailMsg.IsBodyHtml <- true
    mailMsg.Subject <- subject
    mailMsg.Body <- body

    for att in attachments do
        let attachment = new Attachment(att)
        mailMsg.Attachments.Add(attachment)
    let mailClient = new SmtpClient()
    mailClient.Host <- ConfigurationManager.AppSettings.["SMTPMailHost"].Trim()
    mailClient.Port <- Convert.ToInt32(ConfigurationManager.AppSettings.["SMTPMailPort"].Trim())
    mailClient.UseDefaultCredentials <- false
    mailClient.Send(mailMsg)


[<EntryPoint>]
let main argv = 


    let competitionId = ConfigurationManager.AppSettings.["CompetitionId"].Trim()
    let divisionId = ConfigurationManager.AppSettings.["DivisionId"].Trim()
    let stageId = ConfigurationManager.AppSettings.["StageId"].Trim()
    let teamId = ConfigurationManager.AppSettings.["TeamId"].Trim()

    let summaryTemplateFile = ConfigurationManager.AppSettings.["SummaryTemplateFile"].Trim()
    let teamListItemTemplateFile = ConfigurationManager.AppSettings.["TeamListItemTemplateFile"].Trim()
    let playerListItemTemplateFile = ConfigurationManager.AppSettings.["PlayerListItemTemplateFile"].Trim()
    let summaryHeaderFile = ConfigurationManager.AppSettings.["SummaryHeaderFile"].Trim()
   
    let fromEmailAddress = ConfigurationManager.AppSettings.["FromEmailAddress"].Trim()
    let fromEmailName = ConfigurationManager.AppSettings.["FromEmailName"].Trim()
    let toEmailAddress = ConfigurationManager.AppSettings.["ToEmailAddress"].Trim()

    //let teamDetail = readTeamDetail(competitionId, divisionId, stageId, teamId)
    //let teamMatches = readTeamMatchHistory(competitionId, divisionId, stageId, teamId)
    //let teamPlayers = readTeamPlayerResult(competitionId, divisionId, stageId, teamId)
    //let shopDetail = readShopDetail("59399")

    let teams = readTeams(competitionId, divisionId, stageId, teamId)

    let ourTeam = readTeamDetail(competitionId, divisionId, stageId, teamId).[0]
    let ourTeamPlayers = readTeamPlayers(competitionId, divisionId, stageId, teamId)

    let teamMatches = readTeamMatchHistory(competitionId, divisionId, stageId, teamId)
    let nextMatch = findNextMatch(teamMatches)
    let nextAgaistTeamId = if (teamId=nextMatch.teamAId) then nextMatch.teamBId else nextMatch.teamAId
    let nextMatchShopId = if (nextMatch.shpAId=nextMatch.shpBId) then nextMatch.shpAId else ""
    
    
    let nextAgainstTeam = readTeamDetail(competitionId, divisionId, stageId, nextAgaistTeamId).[0]
    let nextAgainstTeamPlayers = readTeamPlayers(competitionId, divisionId, stageId, nextAgaistTeamId)
    let nextMatchShop = if (nextMatchShopId<>"") then readShopDetail(nextMatchShopId).[0] else NullShop

    let summaryTemplate = readFromTextFile(summaryTemplateFile)
    let teamListItemTemplate = readFromTextFile(teamListItemTemplateFile)
    let playerListItemTemplate = readFromTextFile(playerListItemTemplateFile)
    let summaryHeaderHtml = readFromTextFile(summaryHeaderFile)


    let teamListHtml = 
        teams
            |> Array.map(fun team ->
                                    let teamHtml = teamListItemTemplate
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.rankNum}", HttpUtility.HtmlEncode(team.rankNum.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.teamId}", HttpUtility.HtmlEncode(team.teamId.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.teamNm}", HttpUtility.HtmlEncode(team.teamNm.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.rtg}", HttpUtility.HtmlEncode(team.rtg.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.ppd}", HttpUtility.HtmlEncode(team.ppd.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.mpr}", HttpUtility.HtmlEncode(team.mpr.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.totalPt}", HttpUtility.HtmlEncode(team.totalPt.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.matchWin}", HttpUtility.HtmlEncode(team.matchWin.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.matchLose}", HttpUtility.HtmlEncode(team.matchLose.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.setWin}", HttpUtility.HtmlEncode(team.setWin.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.setLose}", HttpUtility.HtmlEncode(team.setLose.ToString()))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.setRatio}", HttpUtility.HtmlEncode(team.setRatio.ToString()+"%"))
                                    let teamHtml  = teamHtml.Replace("{TeamSummary.imageSource}", if (team.imgMd5Txt.Length>0) then 
                                                                                                        HttpUtility.HtmlEncode("http://play.phoenixdart.com/downloadTeamInforImage.do?teamId=" + team.teamId)
                                                                                                  else
                                                                                                        HttpUtility.HtmlEncode("http://images.phoenixdart.com/pdcs/front/images/no_team_image.gif")
                                                                    )
                                    teamHtml
                        )
            |> String.concat("\n")


    let againstPlayerListHtml = 
        nextAgainstTeamPlayers
            |> Array.map(fun player ->
                                    let playerHtml = playerListItemTemplate
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.rankNum}", HttpUtility.HtmlEncode(player.rankNum.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.plyrNm}", HttpUtility.HtmlEncode(player.plyrNm.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.rtg}", HttpUtility.HtmlEncode(player.rtg.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.ppd}", HttpUtility.HtmlEncode(player.ppd.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.mpr}", HttpUtility.HtmlEncode(player.mpr.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setTotal}", HttpUtility.HtmlEncode(player.setTotal.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setWin}", HttpUtility.HtmlEncode(player.setWin.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setLose}", HttpUtility.HtmlEncode(player.setLose.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setRatio}", HttpUtility.HtmlEncode(player.setRatio.ToString()+"%"))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.imageSource}", HttpUtility.HtmlEncode("http://www.phoenixdart.com/hk/member/getmemberphoto?c_seq=" + player.cSeq))
                                    playerHtml
                        )
            |> String.concat("\n")


    let ourPlayerListHtml = 
        ourTeamPlayers
            |> Array.map(fun player ->
                                    let playerHtml = playerListItemTemplate
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.rankNum}", HttpUtility.HtmlEncode(player.rankNum.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.plyrNm}", HttpUtility.HtmlEncode(player.plyrNm.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.rtg}", HttpUtility.HtmlEncode(player.rtg.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.ppd}", HttpUtility.HtmlEncode(player.ppd.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.mpr}", HttpUtility.HtmlEncode(player.mpr.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setTotal}", HttpUtility.HtmlEncode(player.setTotal.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setWin}", HttpUtility.HtmlEncode(player.setWin.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setLose}", HttpUtility.HtmlEncode(player.setLose.ToString()))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.setRatio}", HttpUtility.HtmlEncode(player.setRatio.ToString()+"%"))
                                    let playerHtml  = playerHtml.Replace("{TeamPlayer.imageSource}", HttpUtility.HtmlEncode("http://www.phoenixdart.com/hk/member/getmemberphoto?c_seq=" + player.cSeq))
                                    playerHtml
                        )
            |> String.concat("\n")


    let summaryHtml = summaryTemplate
    let summaryHtml = summaryHtml.Replace("{competitionId}", HttpUtility.HtmlEncode(competitionId.ToString()))
    let summaryHtml = summaryHtml.Replace("{divisionId}", HttpUtility.HtmlEncode(divisionId.ToString()))
    let summaryHtml = summaryHtml.Replace("{stageId}", HttpUtility.HtmlEncode(stageId.ToString()))
    let summaryHtml = summaryHtml.Replace("{OurTeamDetail.teamId}", HttpUtility.HtmlEncode(ourTeam.teamId.ToString()))
    let summaryHtml = summaryHtml.Replace("{OurTeamDetail.teamNm}", HttpUtility.HtmlEncode(ourTeam.teamNm.ToString()))
    let summaryHtml = summaryHtml.Replace("{OurTeamDetail.rankNum}", HttpUtility.HtmlEncode(ourTeam.rankNum.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.teamId}", HttpUtility.HtmlEncode(nextAgainstTeam.teamId.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.teamNm}", HttpUtility.HtmlEncode(nextAgainstTeam.teamNm.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.rankNum}", HttpUtility.HtmlEncode((nextAgainstTeam.rankNum.ToString())))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.cptNm}", HttpUtility.HtmlEncode(nextAgainstTeam.cptNm.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.shpNm}", HttpUtility.HtmlEncode(nextAgainstTeam.shpNm.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.totalPt}", HttpUtility.HtmlEncode(nextAgainstTeam.totalPt.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.teamRtg}", HttpUtility.HtmlEncode(nextAgainstTeam.teamRtg.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.teamPpd}", HttpUtility.HtmlEncode(nextAgainstTeam.teamPpd.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.teamMpr}", HttpUtility.HtmlEncode(nextAgainstTeam.teamMpr.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.setWin}", HttpUtility.HtmlEncode(nextAgainstTeam.setWin.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.setLose}", HttpUtility.HtmlEncode(nextAgainstTeam.setLose.ToString()))
    let summaryHtml = summaryHtml.Replace("{AgainstTeamDetail.setRatio}", HttpUtility.HtmlEncode(nextAgainstTeam.setRatio.ToString()+"%"))
    let summaryHtml = summaryHtml.Replace("{NextMatch.plyngStrtDt}", HttpUtility.HtmlEncode(nextMatch.plyngStrtDt.ToString()))
    let summaryHtml = summaryHtml.Replace("{NextMatch.brcktId}", HttpUtility.HtmlEncode(nextMatch.brcktId.ToString()))
    let summaryHtml = summaryHtml.Replace("{NextMatchShop.name}", HttpUtility.HtmlEncode(nextMatchShop.name.ToString()))
    let summaryHtml = summaryHtml.Replace("{NextMatchShop.address}", HttpUtility.HtmlEncode(nextMatchShop.address.ToString()))
    let summaryHtml = summaryHtml.Replace("{NextMatchShop.latitude}", HttpUtility.HtmlEncode(nextMatchShop.latitude.ToString()))
    let summaryHtml = summaryHtml.Replace("{NextMatchShop.longitude}", HttpUtility.HtmlEncode(nextMatchShop.longitude.ToString()))
    let summaryHtml = summaryHtml.Replace("{NextMatchShop.sSeq}", HttpUtility.HtmlEncode(nextMatchShop.sSeq.ToString()))
    let summaryHtml = summaryHtml.Replace("{teamListItems}", teamListHtml)
    let summaryHtml = summaryHtml.Replace("{againstTeamPlayerListItems}", againstPlayerListHtml)
    let summaryHtml = summaryHtml.Replace("{ourTeamPlayerListItems}", ourPlayerListHtml)
    let summaryHtml = summaryHtml.Replace("{teamSummaryHeader}", summaryHeaderHtml)
    


    //Write to Html
    try
        let sw = new System.IO.StreamWriter("C:\\Temp\\POLSummaryOutput.html")
                                
        sw.WriteLine(summaryHtml)
        sw.Close()
    with 
        | exn -> 
            Console.WriteLine(String.Format("Error writing header trade to file/stream!"))


    let mailSubject = String.Format("{0} - {1} vs {2}, {3}", nextMatch.cpttnNm, ourTeam.teamNm, nextAgainstTeam.teamNm, nextMatch.plyngStrtDt)

    SendSMTPMail (new MailAddress(fromEmailAddress, fromEmailName)) toEmailAddress mailSubject summaryHtml [||]





    0 // return an integer exit code
