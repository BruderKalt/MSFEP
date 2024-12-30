namespace MSFEP.Models;
public static class Templates
{
    public static object GetMSFCostCenterTemplate(string givenState)
    {
        var verificationRequest = new
        {
            includeQRCode = true,
            includeReceipt = true,
            authority = "did:web:mustersoftwarefabrik.com",
            registration = new
            {
                clientName = "MSFEP"
            },
            callback = new
            {
                url = "https://7400-2003-c5-720-ffa7-b1c4-a29a-a053-5e36.ngrok-free.app/verification/presentationCallback",
                //url = "https://mustersoftwarefabrik.com/verification/presentationCallback",
                state = givenState
            },
            requestedCredentials = new[]
               {
                new
                {
                    type = "MSFCostCenter",
                    purpose = "Autorisierung der Zeiteintragsbuchung.",
                    acceptedIssuers = new[]
                    {
                        "did:web:mustersoftwarefabrik.com"
                    },
                    configuration = new
                    {
                        validation = new
                        {
                            allowRevoked = false,
                            validateLinkedDomain = false
                        }
                    }
                }
            }
        };

        return verificationRequest;
    }
    public static object GetMSFProjectAffiliationTemplate(string givenState)
    {
        var verificationRequest = new
        {
            includeQRCode = true,
            includeReceipt = true,
            authority = "did:web:mustersoftwarefabrik.com",
            registration = new
            {
                clientName = "MSFEP"
            },
            callback = new
            {
                url = "https://7400-2003-c5-720-ffa7-b1c4-a29a-a053-5e36.ngrok-free.app/verification/presentationCallback",
                //url = "https://mustersoftwarefabrik.com/verification/presentationCallback",
                state = givenState
            },
            requestedCredentials = new[]
                    {
                new
                {
                    type = "MSFProjectAffiliation",
                    purpose = "Autorisierung der Team-Zugehörigkeit.",
                    acceptedIssuers = new[]
                    {
                        "did:web:mustersoftwarefabrik.com"
                    },
                    configuration = new
                    {
                        validation = new
                        {
                            allowRevoked = false,
                            validateLinkedDomain = false
                        }
                    }
                }
            }
        };

        return verificationRequest;
    }
    public static object GetVerifiedEmployeeTemplate(string givenState)
    {
        var verificationRequest = new
        {
            includeQRCode = true,
            includeReceipt = true,
            authority = "did:web:mustersoftwarefabrik.com",
            registration = new
            {
                clientName = "MSFEP"
            },
            callback = new
            {
                url = "https://7400-2003-c5-720-ffa7-b1c4-a29a-a053-5e36.ngrok-free.app/verification/presentationCallback",
                //url = "https://mustersoftwarefabrik.com/verification/presentationCallback",
                state = givenState
            },
            requestedCredentials = new[]
                    {
                new
                {
                    type = "VerifiedEmployee",
                    purpose = "Autorisierung des Anstellungsverhältnisses für den Zugriff auf MSFEP.",
                    acceptedIssuers = new[]
                    {
                        "did:web:mustersoftwarefabrik.com"
                    },
                    configuration = new
                    {
                        validation = new
                        {
                            allowRevoked = false,
                            validateLinkedDomain = false
                        }
                    }
                }
            }
        };

        return verificationRequest;
    }
}