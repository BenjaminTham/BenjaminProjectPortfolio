import smtplib
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from os import getenv

smtp_server = getenv("SMTP_SERVER")
smtp_port = getenv("SMTP_PORT")
smtp_login = getenv("SMTP_LOGIN")
smtp_password = getenv("SMTP_PASSWORD")

def send_email(to, subject, body):
    try:
        from_email = "GateGuard Security <alert@gateguard.spmovy.com>"
        message = MIMEMultipart()
        message["From"] = from_email
        message["To"] = ", ".join(to)
        message["Subject"] = subject
        message.attach(MIMEText(body))

        server = smtplib.SMTP(smtp_server, int(smtp_port))
        server.starttls()
        server.login(smtp_login, smtp_password)
        server.sendmail(from_email, to, message.as_string())
        server.quit()

        print("[Alert] Email sent successfully")

    except Exception as e:
        print(f"[Alert] Unable to send email - {e}")
