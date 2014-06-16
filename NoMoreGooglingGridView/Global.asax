<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Codice eseguito all'avvio dell'applicazione

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Codice eseguito all'arresto dell'applicazione

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Codice eseguito in caso di errore non gestito

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Codice eseguito all'avvio di una nuova sessione

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Codice eseguito al termine di una sessione. 
        // Nota: l'evento Session_End viene generato solo quando la modalità sessionstate
        // è impostata su InProc nel file Web.config. Se la modalità è impostata su StateServer 
        // o SQLServer, l'evento non viene generato.

    }
       
</script>
