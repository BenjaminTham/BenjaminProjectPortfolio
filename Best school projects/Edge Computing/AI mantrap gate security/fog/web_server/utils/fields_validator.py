from re import match

import psycopg2
from bcrypt import hashpw, gensalt
from email_validator import validate_email, EmailNotValidError


def validate_fields(fields, json, cursor = None):

    data, errors = {}, {}

    for field in fields:
        field_name = field.get("field_name")
        pretty_name = field.get("pretty_name", field_name.replace("_", " ").capitalize())
        field_type = field.get("type")
        error_message = field.get("error_message")
        value = json.get(field_name, None)

        # Handle empty fields
        if value is None:
            if field.get("required"):
                errors[field_name] = error_message or f"{pretty_name} is required"
            continue

        # Check strings
        if field_type == "string":

            # Check datatype
            if not isinstance(value, str):
                errors[field_name] = error_message or f"{pretty_name} must be a string"
                continue

            # Trim string
            if field.get("trim"):
                value = value.strip()

            # Check minimum length
            if field.get("min") and len(value) < field.get("min"):
                errors[field_name] = error_message or f"{pretty_name} must be at least {field.get('min')} characters"

            # Check maximum length
            if field.get("max") and len(value) > field.get("max"):
                errors[field_name] = error_message or f"{pretty_name} must not be longer than {field.get('max')} characters"

            # Check regex
            if field.get("regex") and not match(field.get("regex"), value):
                errors[field_name] = error_message or f"{pretty_name} is not in the correct format"

            # Check email format
            if "email" in field.get("special", []):
                try:
                    validate_email(value)
                except EmailNotValidError:
                    errors[field_name] = error_message or f"{value} is not a valid email address"

            # Check password complexity
            if ("password_complexity" in field.get("special", [])
                    and not match('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?":{}|<>])[A-Za-z\\d!@#$%^&*(),.?":{}|<>]{8,32}$', value)):
                errors[field_name] = error_message or f"{pretty_name} does not meet complexity requirements"

            # Hash password
            if "hash_password" in field.get("special", []):
                value = hashpw(value.encode("utf-8"), gensalt()).decode()

        # Check integers
        if field_type == "integer":

            # Check datatype
            if not isinstance(value, int):
                errors[field_name] = error_message or f"{pretty_name} must be an integer"

            # Check minimum value
            if field.get("min") and value < field.get("min"):
                errors[field_name] = error_message or f"{pretty_name} must be at least {field.get('min')}"

            # Check maximum value
            if field.get("max") and value > field.get("max"):
                errors[field_name] = error_message or f"{pretty_name} must be not be greater than {field.get('max')}"

        # Check booleans
        if field_type == "boolean":

            # Check datatype
            if not isinstance(value, bool):
                errors[field_name] = error_message or f"{pretty_name} must be a boolean"

        # Check allowed values
        if field.get("in") and value not in field.get("in"):
            errors[field_name] = error_message or f"{pretty_name} must be one of the following: {', '.join(field.get('in'))}"

        if unique := field.get("unique"):
            table_name = unique.get("table_name")
            column_name = unique.get("column_name")
            entry_id = unique.get("id")
            query = f"SELECT COUNT(id) as count FROM {table_name} WHERE {column_name} = %s"
            params = [value]
            # Do not match with itself
            if entry_id is not None:
                query += f" AND id != %s"
                params.append(entry_id)
            cursor.execute(query, tuple(params))
            if cursor.fetchone()["count"] != 0:
                errors[field_name] = error_message or f"{pretty_name} already exists"

        data[field.get("output_name", field_name)] = value

    return data, errors
