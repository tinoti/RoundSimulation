﻿Glavni program stvara novi helpers objekt, te zove metodu Round(), koja simulira jednu rundu.

public List<Entity> LoadDataFromJson()
	
	Koristi JSON.NET framework kako bi učitao podatke iz JSON datoteke u Entity objekte. Vraća listu Entity objekata. JSON datoteka
	se mora nalaziti u Test\Test\bin\Debug folderu projekta.


public void DrawBoard(List<Entity> entities)
	
	Ispisuje trenutne atribute svih entiteta na konzolu. 


public Entity SelectSource(List<Entity> entities)
	
	Prompta korisnika za upis id-a napadača, traži ga u listi te ga vraća. Provjerava attackReady entiteta.


public Entity SelectTarget(List<Entity> entities, Entity sourceEntity)
	
	Prompta korisnika za upisa id-a mete, traži ga u listi te ga vraća. Mora uzimati sourceEntity radi provjere je li napadač Creature (ako je creature ne može
	napadati avatara)


public int CheckModifiers(List<Modifiers> modifiers)
	
	Vraća ukupni rezultat modifikatora. Oduzima za Armor te dodaje za Vulnerability.


public int Attack(Entity sourceEntity, Entity targetEntity)

	Simulira jedan napad, zajedno sa retaliationom. Vraća ukupnu vrijednost napada.


public void SaveAttackResultToEntityList(List<Entity> entities, Entity sourceEntity, Entity targetEntity)
	
	Zamjenjuje postojeće objekte u listi sa promjenjenim objektima nakon napada
	

public void SaveDataToJson(List<Entity> entities)
	
	Sprema listu Entity objekata u json file (path je isti kao kod loadanja JSON-a u objekt)
