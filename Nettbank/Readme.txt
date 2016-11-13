Utviklet av:
Martin Veshovda Løland, s236323
Odd Magnus Meyer, s236639
=====================
Oblig 1
---
Dersom det skulle oppstå noen databaseproblemer, så er det to knapper på hjemmesiden: Slett DB og Autogenerer.
Disse to knappene utfører henholdsvis følgende:  Sletter databasen (Bank1), og oppretter to testkunder med hver sine to kontoer 
og noen tilhørende transaksjoner.
Innloggingsinformasjon for testkundene:
Personnummer: 9999911111 og 99999222222
Passord: test
BankID: 123456

Som man kan se i noen av .cshtml-filene, som f.eks. i Transaksjon/visTransaksjon.cshtml og i Transaksjon/RegistrerBetaling.cshtml, 
så har vi i bunnen av filene gjort (utkommenterte) forsøk på bruk av Ajax og JSON. Vi tok en avgjørelse om å legge Ajax og 
JSON til side til oblig 2 for å fokusere på funksjonalitet i denne obligen. I toppen av navbaren er det to linker til 
"Kundeliste" og "Opprett Kunde". Disse er der for test-funksjonalitet i denne omgang, og vil brukes videre til admin-forbruk 
i oblig 2.

=====================
Oblig 2
---
Databasen vil nå autogeneres sammen med en Admin om denne ikke allerede eksisterer. Dette ble gjort slik at vi kunne fjerne 
knappene for å autogenere to kunder og slette databasen for utseendes skyld. Hvis det er ønskelig å benytte de gamle knappene
for å automatisk ha et par kunder, samt kontoer og registerte transaksjoner må man fjerne kommenteringen i klassen 
KundeController fra linje 718 til 936. I tillegg for å få tak i knappene som utfører disse metodene må kommenteringen i viewet
Kunde/Index.cshtml fra linje 112 til 127 fjernes.(OBS denne metoden generer en admin i tillegg til to kunder så det anbefales 
å slette databasen før man bruker metoden).
---
Har her tatt i bruk TrackingEnabledDbContext gjennom NuGet packages for å loggføre endringer i databasen. 
Denne gjør at endringer i databasen blir loggført til to relasjonstabeller i databasen; AuditLog og AuditLogDetail.
For å bruke dette biblioteket brukes "TrackerContext" istedenfor "DbContext" i DBContext.cs, tabeller som skal loggføres 
tagges med [TrackChanges], og individuelle felter hoppes eventuelt over med taggen [SkipTracking]. Se forøvrig 
https://github.com/bilal-fazlani/tracker-enabled-dbcontext/wiki for dokumentasjon.
---
Versjonskontroll: Vi har brukt Github og Github VS extension: https://github.com/Loellis/Nettbank/
Autogenererte tester: Har brukt IntelliTest i VS Enterprise
---
Admin-login:
Personnummer:00000000000
Passord:admin
BankID: 123456

