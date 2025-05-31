from telegram import Update
from sklearn.tree import DecisionTreeClassifier as SklearnDTC
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score
import pandas as pd
import numpy as np
from telegram import ReplyKeyboardMarkup

class MyDecisionTreeClassifier:

    def __init__(self):
        self.questions = [
            "Did the caller verify their identity?",
            "Did the caller ask for personal information?",
            "Did the caller use urgency tactics?",
            "Did the caller have unusual requests or behavior?",
            "Did the caller follow verification procedures?",
            "Were there any spelling or grammar mistakes?"
        ]
        self.responses = []
        self.index = 0
        self.dt_model = self.train_model()

    def start_quiz(self, update, context):
        # Define a custom keyboard
        custom_keyboard = [['Yes', 'No']]
        reply_markup = ReplyKeyboardMarkup(custom_keyboard)
        # Send a message with the custom keyboard
        context.bot.send_message(chat_id=update.effective_chat.id, text=self.questions[self.index], reply_markup=reply_markup)

    def collect_response(self, update, context):
        # Here, we assume the responses are 'yes' or 'no', and map them to 1 and 0 respectively
        response = 1 if update.message.text.lower() == 'yes' else 0
        self.responses.append(response)
        self.index += 1
        if self.index < len(self.questions):
            self.start_quiz(update, context)  # Send next question

    def is_quiz_complete(self):
        return self.index == len(self.questions)

    def analyze_responses(self):
        pred = self.dt_model.predict([self.responses])
        self.reset()
        
        if pred[0] == 1:
            return "🚨 *Detective Mode On!* 🚨 Based on your responses in the quiz, it seems you're dealing with a potential vishing attempt! Stay alert and be sure to protect your personal information."
        else:
            return "🎉 *Good News!* 🎉 Based on your responses, it seems that the situation you're dealing with is safe. However, always stay alert!"


    def reset(self):
        self.responses = []
        self.index = 0

    def train_model(self):
        df = pd.read_csv('resources/quiz_dataset.csv')
        X = df.iloc[:, :-1].values
        y = df.iloc[:, -1].values

        X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=1)
        model = SklearnDTC()
        model.fit(X_train, y_train)

        y_pred = model.predict(X_test)
        print('Accuracy: ', accuracy_score(y_test, y_pred))

        return model