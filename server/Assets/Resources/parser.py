inp = file('lexicon.txt', 'r')
lexicon = inp.readlines()
s = set([''])
for i in range(0, 10000):
	lexicon[i] = lexicon[i].replace('\n', '')
	s.add(lexicon[i].split(' ')[0])
inp.close()
inp = file('phrases.txt', 'r')
oup = file('output.txt', 'w')
phrases = inp.readlines()
for i in range(0, len(phrases)):
	phrases[i] = phrases[i].replace('\n', '')
	words = phrases[i].split(' ')
	flag = True
	for j in range(0, len(words)):
		if words[j].lower() not in s:
			flag = False
			break
	if flag == True:
		oup.write(phrases[i] + '\n')
oup.close()
