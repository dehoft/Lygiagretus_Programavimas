#include <iostream>
#include <fstream>
#include <string>
#include <iomanip>
#include <thread>


using namespace std;

int masyvo_dydis;	// globalus kintamasis skirtas saugoti rezultatų masyvo dydį
string duomenys = "DeksnysE_L1a_dat.txt";
string rezultatai = "DeksnysE_L1a_rez.txt";

struct Studentas
{
	string vardas;
	int gimMetai;
	double stojimoBalas;

};

struct Info
{
	string grupe;
	int kiekis;

};

struct Rezultatai
{
	int nr;
	string pav;
	string vardas;
	int gimMetai;
	double stojimoBalas;
};

void Skaitymas(Studentas A1[], Studentas A2[], Studentas A3[], Studentas A4[], Studentas A5[], Info I[]);
void RasytiDuom(Studentas A1[], Studentas A2[], Studentas A3[], Studentas A4[], Studentas A5[], Info info[]);
void RasytiRez(Rezultatai Rez[]);
void Pildyti(Rezultatai Rezz[], Studentas Stud[], int n, string gija);

int main()
{
	Studentas A1[15], A2[15], A3[15], A4[15], A5[15];
	Info I[5];
	Rezultatai Rez[75];
	masyvo_dydis = 0;

	Skaitymas(A1, A2, A3, A4, A5, I);
		
	RasytiDuom(A1, A2, A3, A4, A5, I);
	thread gija_1(Pildyti, Rez, A1, I[0].kiekis, "gija_1");
	thread gija_2(Pildyti, Rez, A2, I[1].kiekis, "gija_2");
	thread gija_3(Pildyti, Rez, A3, I[2].kiekis, "gija_3");
	thread gija_4(Pildyti, Rez, A4, I[3].kiekis, "gija_4");
	thread gija_5(Pildyti, Rez, A5, I[4].kiekis, "gija_5");

	gija_1.join();
	gija_2.join();
	gija_3.join();
	gija_4.join();
	gija_5.join();
	cout << Rez[1].pav << endl;
	RasytiRez(Rez);
    return 0;
}



/// <summary>
/// Metodas kuris nuskaito duomenis į 5 skirtingus masyvus
/// </summary>
/// <param name="A1"></param>
/// <param name="A2"></param>
/// <param name="A3"></param>
/// <param name="A4"></param>
/// <param name="A5"></param>
/// <param name="I"></param> Masyvas skirtas saugoti informacijai apie kiekvieną studentų grupę
void Skaitymas(Studentas A1[], Studentas A2[], Studentas A3[], Studentas A4[], Studentas A5[], Info I[])
{
	ifstream read(duomenys);
	for (int i = 0; i < 5; i++)
	{
		string grupe;
		int kiekis;
		Info info;
		read >> grupe >> kiekis;
		info.grupe = grupe;
		info.kiekis = kiekis;
		
		I[i] = info;
		switch (i) {
		case 0:
			for (int j = 0; j < kiekis; j++) {
				string vardas;
				int metai;
				double balas;
				Studentas a;
				read >> vardas >> metai >> balas;
				
				a.vardas = vardas;
				a.gimMetai = metai;
				a.stojimoBalas = balas;
				A1[j] = a;
			}
			break;
		case 1:
			for (int j = 0; j < kiekis; j++) {
				string vardas;
				int metai;
				double balas;
				Studentas a;
				read >> vardas >> metai >> balas;
				
				a.vardas = vardas;
				a.gimMetai = metai;
				a.stojimoBalas = balas;
				A2[j] = a;
			}
			break;
		case 2:
			for (int j = 0; j < kiekis; j++) {
				string vardas;
				int metai;
				double balas;
				Studentas a;
				read >> vardas >> metai >> balas;
				a.vardas = vardas;
				a.gimMetai = metai;
				a.stojimoBalas = balas;
				A3[j] = a;
			}
			break;
		case 3:
			for (int j = 0; j < kiekis; j++) {
				string vardas;
				int metai;
				double balas;
				Studentas a;
				read >> vardas >> metai >> balas;
				a.vardas = vardas;
				a.gimMetai = metai;
				a.stojimoBalas = balas;
				A4[j] = a;
			}
			break;
		case 4:
			for (int j = 0; j < kiekis; j++) {
				string vardas;
				int metai;
				double balas;
				Studentas a;
				read >> vardas >> metai >> balas;
				a.vardas = vardas;
				a.gimMetai = metai;
				a.stojimoBalas = balas;
				A5[j] = a;
			}
		}
	}
}

	/// <summary>
	/// metodas skirtas išspausdinti nuskaitytus duomenis į rezultatų failą
	/// </summary>
	/// <param name="A1"></param>
	/// <param name="A2"></param>
	/// <param name="A3"></param>
	/// <param name="A4"></param>
	/// <param name="A5"></param>
	/// <param name="info"></param>
	void RasytiDuom(Studentas A1[], Studentas A2[], Studentas A3[], Studentas A4[], Studentas A5[], Info info[]) {
		ofstream R(rezultatai);
		for (int i = 0; i < 5; i++) {
			R << "*** " << info[i].grupe << " ***" << endl;
			R << setw(10) << right << "Vardas " << setw(7) << right << "Gimimo Metai" << setw(15) << right << "Stojimo balas" << endl;
			for (int j = 0; j < info[i].kiekis; j++) {
				switch (i) {
				case 0:
					R << setw(1) << to_string(j + 1) + ") " << setw(10) << left << A1[j].vardas << setw(15) << A1[j].gimMetai << setw(7) << A1[j].stojimoBalas << endl;
					break;
				case 1:
					R << setw(1) << to_string(j + 1) + ") " << setw(10) << left << A2[j].vardas << setw(15) << A2[j].gimMetai << setw(7) << A2[j].stojimoBalas << endl;
					break;
				case 2:
					R << setw(1) << to_string(j + 1) + ") " << setw(10) << left << A3[j].vardas << setw(15) << A3[j].gimMetai << setw(7) << A3[j].stojimoBalas << endl;
					break;
				case 3:
					R << setw(1) << to_string(j + 1) + ") " << setw(10) << left << A4[j].vardas << setw(15) << A4[j].gimMetai << setw(7) << A4[j].stojimoBalas << endl;
					break;
				case 4:
					R << setw(1) << to_string(j + 1) + ") " << setw(10) << left << A5[j].vardas << setw(15) << A5[j].gimMetai << setw(7) << A5[j].stojimoBalas << endl;
				}

			}
		}
		R << string(40, '-') << endl;
		R.close();
	}

	/// <summary>
	/// Metodas skirtas išspausdinti galutiniuus rezultatus į rezultatų failą
	/// </summary>
	/// <param name="Rez"></param>
	void RasytiRez(Rezultatai Rez[]) {
		ofstream R(rezultatai, ios::app);
		
		for (int i = 0; i < masyvo_dydis; i++) {
			R << Rez[i].pav << " " << Rez[i].nr << " " << setw(20) << left << Rez[i].vardas <<
				setw(5) << left << Rez[i].gimMetai << setw(4) << left << Rez[i].stojimoBalas << endl;
		}
		R << endl << "Pabaiga";
		R.close();
	}


	/// <summary>
	/// Metodas skirtas gijoms iš kelių skirtingų masyvų sudėti duomenis į galutinį rezultatų masyvą
	/// </summary>
	/// <param name="Rezz"></param>
	/// <param name="Stud"></param>
	/// <param name="n"></param>
	/// <param name="gija"></param>
	void Pildyti(Rezultatai Rezz[], Studentas Stud[], int n, string gija) {
		int i = 0;
		Rezultatai Rez;
		while (i < n) {
			Rez.gimMetai = Stud[i].gimMetai;
			
			Rez.vardas = Stud[i].vardas;
			Rez.stojimoBalas = Stud[i++].stojimoBalas;
			Rez.nr = i;
			Rez.pav = gija;
			
			Rezz[masyvo_dydis++] = Rez;
			cout << masyvo_dydis << endl;
			for (int j = 0; j < 10000; j++) {
				double k = sqrt(j);
			}
			
		}
	}