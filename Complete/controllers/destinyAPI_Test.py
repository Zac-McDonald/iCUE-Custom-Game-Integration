import requests

apiKey = '7cb5fac0536840dfbece70b008f688e3'
OAuthURL = 'https://www.bungie.net/en/OAuth/Authorize'
OAuthId = '26153'

'f27baa70628e80a15e87df61af219ae5'

ApiURL = 'https://www.bungie.net/Platform'

query = ''
HEADERS = { 'X-API-Key':  apiKey }

def Request(url):
	r = requests.get(ApiURL + url, headers = HEADERS)
	return r.json()

print(Request('/Destiny2/SearchDestinyPlayer/{membershipType}/{displayName}/'.format(membershipType = 4, displayName = 'MaccaCool')))





#p = requests.post('https://www.bungie.net/Platform/App/OAuth/Token/', params = { 'client_id': 18463660, 'grant_type': 'authorization_code', 'code': 'f27baa70628e80a15e87df61af219ae5' } )

#'204'
#print('/Destiny2/{membershipType}/Profile/{destinyMembershipId}/Character/{characterId}/ '.format(membershipType = '4', destinyMembershipId = 'ABC', characterId = 'Test'))

#response = requests.get(ApiURL, params = info)
#print (response.content)