# 🎯 Project Overview and User Guide

This project aims to detect scam calls using machine learning techniques. It involves data preprocessing, exploratory data analysis, model training, and hyperparameter tuning. This guide provides instructions on how to run the project and the sequence of files to explore.

## 📖 User Guide

1.  **Set up the environment:**

    *   Clone the repository to your local machine.
    *   Navigate to the project directory in your terminal.

        ```bash
        cd INF2008_P1-08
        ```
    *   Create a virtual environment (recommended).

        ```bash
        python -m venv .venv
        ```
    *   Activate the virtual environment.

        *   On Windows:

            ```bash
            .venv\Scripts\activate
            ```
        *   On macOS and Linux:

            ```bash
            source .venv/bin/activate
            ```
    *   Navigate to the `scam_detection` folder.

        ```bash
        cd scam_detection
        ```
    *   Install the required dependencies using the `requirements.txt` file in the `scam_detection` folder.

        ```bash
        pip install -r requirements.txt
        ```

2.  **Exploratory Data Analysis (EDA):**

    *   Navigate to the `scam_detection` folder.
    *   Open the `data_exploration.ipynb` notebook in Jupyter.
    *   Run the notebook to perform exploratory data analysis. This step helps with understanding the dataset better through visualizations and statistical analyses.

3.  **Model Training and Evaluation:**

    *   In the `scam_detection` folder, open the `scam_detection.ipynb` notebook.
    *   Run the notebook to train and evaluate machine learning models for scam detection.
    *   This step involves data pre-processing, feature extraction, data-splitting,  training, and evaluation.

4.  **Hyperparameter Tuning:**

    *   Once the top few models are picked out, the model hyperparameters can be tuned to gain improvements. Open the `hyperparameter-tuning.ipynb` notebook in the `scam_detection` folder.
    *   For this case, the best-performing embedding-algorithm combinations were: **SVM-SBERT, MultiTask-SBERT, MultiTask-BERT**
    *   Run the notebook to perform hyperparameter tuning. This step optimizes the model performance by searching for the best hyperparameter values.
	    * For the multi-task learning model: Uses Ray Tune to optimize hyperparameters like learning rate, batch size, and number of epochs
		* For the SVM model: Uses scikit-learn's GridSearchCV to optimize parameters like C (regularization), kernel type, and gamma

## 📌 Sequence of Files to Explore

1.  **`scam_detection/data_exploration.ipynb`:** To perform exploratory data analysis.
2.  **`scam_detection/scam_detection.ipynb`:** To train and evaluate machine learning models.
3.  **`scam_detection/hyperparameter-tuning.ipynb`:** To fine-tune the model hyperparameters for the multi-task learning model.

## 🤖 Machine Learning Algorithms Explored

The following machine learning algorithms were explored in this project:
* Logistic Regression
* Support Vector Machine (SVM)
* Random Forest
* AdaBoost
* Policy-based Reinforcement Learning (Neural Network)
* Multi-Task Learning (Neural Network)
* Naive Bayes with SVM (NB-SVM)

## 📂 Folder Structure

```
INF2008_P1-08/
├── 📁 scam_detection/
│   ├── 📜 data_exploration.ipynb  # Exploratory Data Analysis (EDA)
│   ├── 📜 scam_detection.ipynb  # Model training & evaluation
│   ├── 📜 hyperparameter-tuning.ipynb  # Hyperparameter tuning
│   ├── 📊 results.csv  # Model evaluation results
│   ├── 📊 tuning_diff_results.csv  # Hyperparameter tuning results
│   ├── 📎 distil_bert_embeddings.npy  # DistilBERT embeddings
│   ├── 📎 sbert_embeddings.npy  # SBERT embeddings
│   └── 📃 requirements.txt  # Dependencies
├── 📁 data/
│   ├── 📊 combined_scam_dataset.csv  # Initial dataset
│   ├── 📊 combined_cleaned_merged_dataset.csv  # Merged dataset with Gemini
│   ├── 📊 combined_scam_dataset_reclassified.csv  # Scam Classes aligned between Gemini and HuggingFace data sources
│   ├── 📊 generic_changed_dataset.csv  # Non-Scam Classes Reclassified to 'generic'
│   ├── 📊 Data merging stats.xlsx  # Data merging stats
│   └── 📜 data_preprocessing.ipynb  # Data preprocessing (For Original HuggingFace data)
│   └── 📃 requirements.txt  # Dependencies
├── 📁 synthetic_data/
│   ├── 📜 synthetic_data_generator.ipynb  # Generate synthetic call logs
│   ├── 📊 cleaned_call_logs.csv  # Cleaned synthetic data
│   ├── 📊 non_scam_call_logs.csv  # Non-scam logs in csv
│   ├── 📊 scam_call_logs.csv  # Scam logs in csv
│   ├── 📃 requirements.txt  # Dependencies
│   └── 📁 raw/
│       ├── 📊 non_scam_call_logs.json  # Raw non-scam data
│       └── 📊 scam_call_logs.json  # Raw scam data
└── 📃 README.md  # Project documentation
```


## 📁 Folder: `scam_detection`

This is the main folder for the project. The flow should be `data_exploration.ipynb` > `scam_detection.ipynb` > `hyperparameter-tuning.ipynb`

-   **data_exploration.ipynb**: This notebook is used for exploratory data analysis (EDA) to understand the merged dataset (Hugging Face + Gemini) better. The notebook includes visualizations and statistical analyses, such as word clouds, n-gram analysis, and frequent word count analysis.
-   **scam_detection.ipynb**: This notebook implements machine learning models for scam detection. It performs the following steps:
    *   Loads the preprocessed data.
    *   Extracts features using CountVectorizer, TF-IDF, Word2Vec, DistilBERT, and SBERT.
    *   Splits the data into training, validation, and test sets.
    *   Trains and evaluates machine learning models for scam detection.
-   **hyperparameter-tuning.ipynb**: This notebook is used for hyperparameter tuning of machine learning models. It uses Ray Tune to optimize hyperparameters for the multi-task neural network model, and scikit-learn's GridSearchCV to optimize the hyperparameters for SVM model.
-   **results.csv**: This file contains the results of the model evaluations as part of training the different embedding-algorithm combinations.
-   **tuning_diff_results.csv**: This file contains the results of the differences calculated during the hyperparameter tuning.
-   **distil_bert_embeddings.npy**: This file contains the DistilBERT embeddings generated from the dataset.
-   **sbert_embeddings.npy**: This file contains the SBERT embeddings generated from the dataset.
-   **requirements.txt**: This file lists the Python dependencies required to run the scripts and notebooks in this directory.

## 📁 Folder: `data`

This folder holds the original and processed Hugging Face data.

#### Dataset Flow
The flow of how the final dataset was derived:

| Step | Filename | Description |
|------|-------------------------------|--------------------------------------------------------------|
| 1️⃣  | **combined_scam_dataset.csv** | The initial HuggingFace dataset. |
| 2️⃣  | **combined_cleaned_merged_dataset.csv** | The dataset post-merging of the synthetic Gemini dataset with the initial Hugging Face dataset, with scam and not scam classes from both data sources preserved and formatted |
| 3️⃣  | **combined_scam_dataset_reclassified.csv** | The dataset post-merging, with scam classes aligned between Gemini and HuggingFace data sources. |
| 4️⃣  | **generic_changed_dataset.csv** | The dataset post-merging, with non-scam classes reclassified to 'generic'. |
| 📊  | **Data merging stats.xlsx** | Statistics and details of the merging and data combination process. |
| 📈  | **data_preprocessing.ipynb** | Exploratory Data Analysis (EDA) of the initial dataset. |


-   **data_preprocessing.ipynb**: This Jupyter notebook handles data ingestion, cleaning, and preprocessing for further use in downstream tasks. This notebook performs the following steps:
    *   Loads data from various CSV files (Hugging Face datasets).
    *   Adds a 'type' column to the dataset based on keywords found in the dialogue.
    *   Combines all datasets with the same columns.
    *   Standardizes speaker labels (e.g., "Suspect", "Innocent") to "Caller" and "Recipient".
    *   Converts the 'dialogue' column to lowercase.
    *   Exports the processed data to a CSV file.
    *   Edit this notebook to tweak data cleaning steps, handle missing values, and perform feature engineering.


## 📁 Folder: `synthetic_data`

This folder holds the Gemini LLM generated data.

-   **synthetic_data_generator.ipynb**: This Jupyter notebook is used to generate synthetic call logs for both scam and non-scam categories. Edit this notebook to change the generation parameters, update the list of scam or legitimate call categories, or modify the JSON structure of the generated logs. To use this file, you need to acquire your own Google Gemini API Key.
-   **cleaned_call_logs.csv**: This file contains the combined and cleaned call logs from both scam and non-scam categories as part of the Gemini LLM generated data. The labels and placeholders have been standardized and cleaned up for further processing.
-   **non_scam_call_logs.csv**: This file contains call logs that are identified as legitimate (non-scam) calls. Each entry includes the dialogue, labels, and type of call.
-   **scam_call_logs.csv**: This file contains call logs that are identified as scam calls. Each entry includes the dialogue, labels, and type of scam.
-   **requirements.txt**: This file lists the Python dependencies required to run the scripts and notebooks in this directory.

### 📁 Subfolder: `synthetic_data/raw`

-   **non_scam_call_logs.json**: This file contains the raw JSON data for legitimate (non-scam) call logs. Each entry includes the call category, language, and dialogue.
-   **scam_call_logs.json**: This file contains the raw JSON data for scam call logs. Each entry includes the scam category, whether the victim was scammed, language, and dialogue.
