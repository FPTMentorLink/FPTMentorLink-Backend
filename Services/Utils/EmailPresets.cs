using Services.Models.Email;

namespace Services.Utils;

public static class EmailPresets
{
    public static EmailContent ProjectInvitationEmail(string invitationLink, string projectName)
    {
        return new EmailContent
        {
            Subject = "Invitation to Join Project - FPTMentorLink",
            HtmlBody = $"""
                        
                                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                                            <div style='text-align: center; margin-bottom: 30px;'>
                                                <h1 style='color: #f26f21;'>FPTMentorLink</h1>
                                            </div>
                                            
                                            <div style='background-color: #f9f9f9; padding: 20px; border-radius: 5px;'>
                                                <h2 style='color: #333; margin-bottom: 20px;'>Project Invitation</h2>
                                                
                                                <p style='color: #555; line-height: 1.6;'>
                                                    You have been invited to join a project <strong>{projectName}</strong> on FPTMentorLink. This invitation link will expire in 24 hours.
                                                </p>
                        
                                                <div style='text-align: center; margin: 30px 0;'>
                                                    <a href='{invitationLink}' 
                                                       style='background-color: #f26f21; 
                                                              color: white; 
                                                              padding: 12px 24px; 
                                                              text-decoration: none; 
                                                              border-radius: 5px; 
                                                              display: inline-block;'>
                                                        Join Project
                                                    </a>
                                                </div>
                        
                                                <p style='color: #555; line-height: 1.6;'>
                                                    If you're unable to click the button, you can copy and paste this link into your browser:
                                                    <br>
                                                    <span style='color: #0066cc;'>{invitationLink}</span>
                                                </p>
                                            </div>
                        
                                            <div style='margin-top: 30px; text-align: center; color: #888; font-size: 12px;'>
                                                <p>This is an automated message from FPTMentorLink. Please do not reply to this email.</p>
                                                <p>© 2025 FPTMentorLink. All rights reserved.</p>
                                            </div>
                                        </div>
                        """
        };
    }
}