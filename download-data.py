types = ["characters", "action_cards", "entities", "keywords"]
languages = ["CHS"]

# download from https://static-data.7shengzhaohuan.online/api/v4/data/v<version>/<language>/<type>
# and store it in data/<version>/<language>/<type>.json
import os
import requests
import time
import sys

for version in sys.argv[1:]:
    for language in languages:
        for type in types:
            url = f"https://static-data.7shengzhaohuan.online/api/v4/data/v{version}/{language}/{type}"
            response = requests.get(url)
            if response.status_code == 200:
                os.makedirs(f"data/{version}/{language}", exist_ok=True)
                with open(
                    f"data/{version}/{language}/{type}.json", "w", encoding="utf-8"
                ) as f:
                    f.write(response.text)
                print(f"Successfully downloaded {url}")
            else:
                print(f"Failed to download {url}, status code: {response.status_code}")
            # sleep for 1 second to avoid overwhelming the server
            time.sleep(1)
