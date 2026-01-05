import openai
import uuid

# openai.api_key = "<OPENAI_API_KEY>"
openai.api_key = "<YOUR_API_KEY_HERE>"
opena_ai_model = "gpt-3.5-turbo"
system_prompt = "You are a translator from English to Serbian."

keys_to_translate = [
'Welcome to Cash Vault',

]


def translate_text(text, target_language="sr"):
    response = openai.ChatCompletion.create(
        model=opena_ai_model,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": f"Translate this to Serbian: '{text}'"}
        ]
    )

    return response.choices[0].message['content'].strip()


# Open the SQL file in append mode
with open("translations.sql", "a", encoding="utf-8") as f:
    for key in keys_to_translate:
        guid_bs = str(uuid.uuid4())
        guid_us = str(uuid.uuid4())

        # Translate the text
        translated_value = translate_text(key)
        translated_value = translated_value.replace("'", "")
        translated_value = translated_value.replace("""""", "")

        sql_bs = f"INSERT INTO \"Message\" (\"Guid\", \"Version\", \"LanguageCode\", \"Key\", \"Value\") VALUES ('{guid_bs}', 1, 'bs', '{key}', '{translated_value}');\n"
        sql_us = f"INSERT INTO \"Message\" (\"Guid\", \"Version\", \"LanguageCode\", \"Key\", \"Value\") VALUES ('{guid_us}', 1, 'us', '{key}', '{key}');\n"

        f.write(sql_bs)
        f.write(sql_us)
        f.write("\n")

print("Translations appended to translations.sql")
