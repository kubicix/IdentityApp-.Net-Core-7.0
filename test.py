import smtplib
import logging
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText

def send_email():
    # Enable logging
    logging.basicConfig(level=logging.DEBUG)
    
    # SMTP server settings
    smtp_server = "smtp-relay.brevo.com"
    smtp_port = 587
    smtp_login = "7a7618002@smtp-brevo.com"
    smtp_password = "xTHDfQEB2K0bWXAy"

    # Email content
    from_email = smtp_login
    to_email = "kubilaybirer@hotmail.com"  # Replace with the recipient's email address
    subject = "Test Email from Python"
    body = "This is a test email sent using Python and Brevo SMTP settings."

    # Create the MIME message
    msg = MIMEMultipart()
    msg["From"] = from_email
    msg["To"] = to_email
    msg["Subject"] = subject

    # Attach the email body
    msg.attach(MIMEText(body, "plain"))

    try:
        # Connect to the SMTP server
        server = smtplib.SMTP(smtp_server, smtp_port)
        server.set_debuglevel(1)  # Enable SMTP debug output
        server.starttls()  # Secure the connection
        server.login(smtp_login, smtp_password)

        # Send the email
        server.sendmail(from_email, to_email, msg.as_string())

        print("Email sent successfully!")

    except Exception as e:
        print(f"Failed to send email: {e}")

    finally:
        # Close the connection to the server
        server.quit()

if __name__ == "__main__":
    send_email()
