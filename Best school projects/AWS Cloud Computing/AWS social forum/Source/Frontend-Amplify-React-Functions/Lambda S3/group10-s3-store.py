import json
import base64
import boto3

def lambda_handler(event, context):
    # Initialize S3 client
    S3 = boto3.client("s3")
    
    s3_bucket = "group-10-s3-bucket"
    
    # Extract base64-encoded content from the event
    get_file_content = event["content"]
    
    # Ensure proper Base64 padding
    padding = len(get_file_content) % 4
    if padding != 0:
        get_file_content += '=' * (4 - padding)
    
    # Decode the content from base64
    try:
        file_content = base64.b64decode(get_file_content)
    except Exception as e:
        return {
            'statusCode': 400,
            'headers': {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Credentials": "true"
            },
            'body': json.dumps(f"Error decoding base64 content: {str(e)}")
        }
    
    # Get the file name
    file_name = event["file_name"]
    
    # Upload the decoded file content to S3
    try:
        s3_upload = S3.put_object(Bucket=s3_bucket, Key=file_name, Body=file_content)
    except Exception as e:
        return {
            'statusCode': 500,
            'headers': {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Credentials": "true"
            },
            'body': json.dumps(f"Error uploading to S3: {str(e)}")
        }

    return {
        'statusCode': 200,
        'headers': {
            "Access-Control-Allow-Origin": "*",
            "Access-Control-Allow-Credentials": "true"
        },
        'body': json.dumps(f'The file {file_name} has been uploaded successfully!')
    }
