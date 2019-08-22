#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include "iostream"
#include "string"
#include "fstream"
#include <thread>
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <mutex>

#include <stdio.h>

using namespace std;

#define DEBUG_MODE 1	// output to console
#define DETAILED_DEBUG 0	// output to console

#if DEBUG_MODE
#define DEBUG
#endif

#if DETAILED_DEBUG
#define DDEBUG
#endif

const char srcfile[] = "../DeksnysE_L4.txt";
const char rezfile[] = "../DeksnysE_L4_rez.txt";
const int datasize = 15;
//const int threadcount = 5;
char gsepLine[] = "+----";
char gnameLine[] = "|Gija";
int gcols = 4;
char sepLine[] = "+---+----+-------------------+--------------------+------------------+--------+\n";
char nameLine[] = "|Nr.| ID | Modelis           | Savininkas         | Pagaminimo metai | Svoris |\n";
int cols[7];

char sepLine2[] = "+---+----+---------------------------------------------------------+-----------------------------------------------------------------+------------------+--------+\n";
char nameLine2[] = "|Nr.| ID | Modelis                                                 | Savininkas                                                      | Pagaminimo metai | Svoris |\n";
int cols2[7];

// class for data to store
class car
{
public:
	char model[100];
	char owner[100];
	int year_made;
	double mass;
	int id;
	string ToString(int num, int col[]) {
		stringstream ss;

		ss << "|" << setw(col[0]) << num;
		ss << "|" << setw(col[1]) << id;
		ss << "|" << left << setw(col[2]);
		if (model != NULL) {
			ss << model;
		}
		else {
			ss << "NULL";
		}
		ss << "|" << left << setw(col[3]);
		if (owner != NULL) {
			ss << owner;
		}
		else {
			ss << "NULL";
		}
		ss << "|" << setw(col[4]) << year_made;
		ss << "|" << setw(col[5]) << mass << "|" << endl;
		string ans = ss.str();
		//ss >> ans;
		return ans;
	}

	car() {}

	car(char model[100], char owner[100], int met, double svo) {
		strcpy(model, model);
		strcpy(owner, owner);
		year_made = met;
		mass = svo;
	}

	static bool compare(car x, car y) {
		return x.year_made < y.year_made;
	}
};

// global variables
const int nrez = 100;
const int n = 12;
const int wt = 6; //writer threads count
const int rt = 6; //reader threads count


car P[wt][n];
car Result[n];
int writersizes[wt];
int readersizes[rt];
ofstream out(rezfile);

// functions 
void readData();
void writeData(char title[], car P[], int size);
void writeResult(char title[], car P[], int size);
void GetColsWidth();


#ifdef DEBUG
void DebugWriteData(char title[], car P[], int size);
void DebugWriteResult(char title[], car P[], int size);
#endif	

/// <summary>
/// reading data from data source file
/// </summary>
void readData() {
	ifstream src(srcfile);
	int size = 0;
	
	for (int i = 0; i < wt; i++) {
		src >> size;
		writersizes[i] = size;
	
		for (int j = 0; j < size; j++)
		{
			char v[100], p[100];

			src >> v >> p >> P[i][j].year_made >> P[i][j].mass; 
			strcpy(P[i][j].model, v);
			strcpy(P[i][j].owner, p);
			P[i][j].id = j + 1;
#ifdef DDEBUG
			cout << P[i][j].name << " " << P[i][j].surname << " " << P[i][j].birth_year << " " << P[i][j].mass << endl;
#endif
		}
		sort(P[i], P[i] + size, (new car)->compare);
	}
	src.close();
}

/// <summary>
/// writes data of array into tables to file 
/// </summary>
/// <param name="title"> title of table </param>
/// <param name="P"> array to print to table </param>
/// <param name="size"> size of data to print to table </param>
void writeData(char title[], car P[], int size) {
#ifdef DEBUG
	DebugWriteData(title, P, size);
#endif
	out << title << endl;
	out << sepLine << nameLine << sepLine;
	for (int i = 0; i < size; i++) {
		out << P[i].ToString(i + 1, cols);
	}
	out << sepLine << endl;
}

/// <summary>
/// writes data of array into tables to file 
/// </summary>
/// <param name="title"> title of table </param>
/// <param name="P"> array to print to table </param>
/// <param name="size"> size of data to print to table </param>
void writeResult(char title[], car P[], int size) {
#ifdef DEBUG
	DebugWriteResult(title, P, size);
#endif
	out << title << endl;
	out << sepLine2 << nameLine2 << sepLine2;
	for (int i = 0; i < size; i++) {
		out << P[i].ToString(i + 1, cols2);
	}
	out << sepLine2 << endl;
}

#ifdef DEBUG
/// <summary>
/// writes data of array into tables to console
/// </summary>
/// <param name="title"> title of table </param>
/// <param name="P"> array to print to table </param>
/// <param name="size"> size of data to print to table </param>
void DebugWriteData(char title[], car P[], int size) {
	cout << title << endl;
	cout << sepLine << nameLine << sepLine;
	for (int i = 0; i < size; i++) {
		cout << P[i].ToString(i + 1, cols);
	}
	cout << sepLine << endl;
}
#endif	

#ifdef DEBUG
/// <summary>
/// writes data of array into tables to console
/// </summary>
/// <param name="title"> title of table </param>
/// <param name="P"> array to print to table </param>
/// <param name="size"> size of data to print to table </param>
void DebugWriteResult(char title[], car P[], int size) {
	cout << title << endl;
	cout << sepLine2 << nameLine2 << sepLine2;
	for (int i = 0; i < size; i++) {
		cout << P[i].ToString(i + 1, cols2);
	}
	cout << sepLine2 << endl;
}
#endif	

/// <summary>
/// get width of table columns for dynamic printing
/// </summary>
void GetColsWidth() {
	int col = 0, size = 0;
	int len = sizeof(sepLine);
	for (int i = 1; i < len; i++) {
		if (sepLine[i] == '+') {
			cols[col] = size;
			size = 0;
			col++;
		}
		else {
			size++;
		}
	}
	col = 0, size = 0;
	len = sizeof(sepLine2);
	for (int i = 1; i < len; i++) {
		if (sepLine2[i] == '+') {
			cols2[col] = size;
			size = 0;
			col++;
		}
		else {
			size++;
		}
	}
}

//////////////////////////////CUDA///////////////////////////

cudaError_t addWithCuda(int *c, const int *a, const int *b, unsigned int size);

__device__ char * my_strcpy(char *dest, const char *src) {
	int i = 0;
	do {
		dest[i] = src[i];
	} while (src[i++] != 0);
	return dest;
}

__device__ char * my_strcat(char *dest, const char *src) {
	int i = 0;
	while (dest[i] != 0) i++;
	my_strcpy(dest + i, src);
	return dest;
}

__global__ void add(car** Pe, car* result) {
	int idx = threadIdx.x;
	result[idx].id = 0;
	result[idx].mass = 0;
	result[idx].year_made = 0;

	for (int i = 0; i < wt; i++)
	{
		result[idx].id += Pe[i][idx].id;
		char *name = result[idx].model;
		my_strcat(name, Pe[i][idx].model);
		char *surname = result[idx].owner;
		my_strcat(surname, Pe[i][idx].owner);
		result[idx].mass += Pe[i][idx].mass;
		result[idx].year_made += Pe[i][idx].year_made;
	}
}

int main()
{
	GetColsWidth();
	readData();
	for (int i = 0; i < wt; i++)
	{
		char pav[18] = "Duomenu rinkinys ";
		char integer_string[1];

		sprintf(integer_string, "%d", i + 1);
		strcat(pav, integer_string);

		writeData(pav, P[i], writersizes[i]);
	}

	//allocate memory on GPU
	int size = n * sizeof(car);
	car **Cuda_P;
	cudaMalloc((void**)&Cuda_P, wt * sizeof(car*));
	//copy
	for (int i = 0; i < wt; i++)
	{
		car * Cuda_P_child;
		cudaMalloc((void**)&Cuda_P_child, size);
		cudaMemcpy(Cuda_P_child, P[i], size, cudaMemcpyHostToDevice);
		cudaMemcpy(&Cuda_P[i], &Cuda_P_child, sizeof(car*), cudaMemcpyHostToDevice);
	}
	car * Cuda_Res;
	cudaMalloc((void**)&Cuda_Res, size);

	// launching proc

	add << <1, n >> >(Cuda_P, Cuda_Res);

	cudaDeviceSynchronize();

	//From GPU to CPU
	cudaMemcpy(Result, Cuda_Res, size, cudaMemcpyDeviceToHost);

	//free up gpu memory
	for (int i = 0; i < wt; i++)
	{
		cudaFree(&Cuda_P[i]);
	}
	cudaFree(Cuda_P);

	writeResult("Resultatas", Result, n);

#ifdef DEBUG
	cout << "program done";
	int a;
	cin >> a;
#endif
	out.close();

	return 0;
}
