versions = ['3.3.0', '3.4.0', '3.5.0', '3.6.0', '3.7.0', '3.8.0', 
    '4.0.0', '4.1.0', '4.2.0', '4.3.0', '4.4.0', '4.5.0', '4.6.0', '4.6.1', '4.7.0', '4.8.0', 
    '5.0.0', '5.1.0', '5.2.0', '5.3.0', '5.4.0', '5.5.0', '5.6.0', '5.7.0', '5.8.0', 
    '6.0.0', '6.1.0', '6.2.0', '6.3.0', '6.4.0']
types = ['characters', 'action_cards', 'entities']
languages = ['CHS']

#download from https://static-data.7shengzhaohuan.online/api/v4/data/v<version>/<language>/<type>
#and store it in data/<version>/<language>/<type>.json
import os
import requests
import time

for version in versions:
    for language in languages:
        for type in types:
            url = f'https://static-data.7shengzhaohuan.online/api/v4/data/v{version}/{language}/{type}'
            response = requests.get(url)
            if response.status_code == 200:
                os.makedirs(f'data/{version}/{language}', exist_ok=True)
                with open(f'data/{version}/{language}/{type}.json', 'w', encoding='utf-8') as f:
                    f.write(response.text)
                print(f'Successfully downloaded {url}')
            else:
                print(f'Failed to download {url}, status code: {response.status_code}')
            #sleep for 1 second to avoid overwhelming the server
            time.sleep(1)
