import json
import boto3
import base64

def retrieve_file_from_s3(partial_name):
    # Create an S3 client
    S3 = boto3.client("s3")
    
    # For testing, you are hardcoding the partial name.
    # In production, you would use the passed-in partial_name.
    #partial_name = "21_45"
    
    # S3 bucket name
    bucket_name = "group-10-s3-bucket"
    
    try:
        # List objects in the S3 bucket with the given prefix
        response = S3.list_objects_v2(Bucket=bucket_name, Prefix=partial_name)
        
        # If no objects are found, return a 404 with CORS headers
        if 'Contents' not in response:
            return {
                'statusCode': 404,
                'headers': {
            'Access-Control-Allow-Headers': 'Content-Type',
            'Access-Control-Allow-Origin': "*",
            'Access-Control-Allow-Credentials' : True,
            'Access-Control-Allow-Methods': 'OPTIONS,POST,GET'
                },
                'body': json.dumps(f"No file found with partial name: {partial_name}")
            }
        
        # Get the first object key from the list
        file_key = response['Contents'][0]['Key']
        
        # Retrieve the file from S3
        s3_object = S3.get_object(Bucket=bucket_name, Key=file_key)
        file_content = s3_object['Body'].read()
        
        # Encode the file content as base64
        encoded_content = base64.b64encode(file_content).decode('utf-8')
        
        return {
            'statusCode': 200,
            'headers': {
            'Access-Control-Allow-Headers': 'Content-Type',
            'Access-Control-Allow-Origin': "*",
            'Access-Control-Allow-Credentials' : True,
            'Access-Control-Allow-Methods': 'OPTIONS,POST,GET'
            },
            'body': json.dumps({
                'message': f"File {file_key} retrieved successfully!",
                'file_content': encoded_content
            })
        }
    
    except Exception as e:
        return {
            'statusCode': 500,
            'headers': {
            'Access-Control-Allow-Headers': 'Content-Type',
            'Access-Control-Allow-Origin': "*",
            'Access-Control-Allow-Credentials' : True,
            'Access-Control-Allow-Methods': 'OPTIONS,POST,GET'
            },
            'body': json.dumps(f"Error retrieving the file: {str(e)}")
        }

def lambda_handler(event, context):
    # Check if the event has a body (as a JSON string) and parse it.
    if "body" in event:
        try:
            body = json.loads(event["body"])
        except Exception as e:
            return {
                'statusCode': 400,
            'headers': {
            'Access-Control-Allow-Headers': 'Content-Type',
            'Access-Control-Allow-Origin': "*",
            'Access-Control-Allow-Credentials' : True,
            'Access-Control-Allow-Methods': 'OPTIONS,POST,GET'
            },
                'body': json.dumps(f"Invalid request body: {str(e)}")
            }
        partial_name = body.get("partial_name", "")
    else:
        partial_name = event.get("partial_name", "")
        
    return retrieve_file_from_s3(partial_name)

