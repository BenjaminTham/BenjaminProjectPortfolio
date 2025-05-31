import os
import importlib
import threading
import pandas as pd
from telegram import Update, ReplyKeyboardMarkup, ReplyKeyboardRemove, InlineKeyboardButton, InlineKeyboardMarkup
from telegram.ext import Updater, CommandHandler, MessageHandler, Filters, CallbackContext, ConversationHandler, CallbackQueryHandler
import speech_recognition as sr
from detection_modules.quiz_decision_tree import MyDecisionTreeClassifier
import time
import logging

# variables
TOKEN = os.environ.get("TELEGRAM_ID")
NAME = 'inf1002-python-project'

# heroku port
PORT = os.environ.get('PORT', 443)

# Enable logging
logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
                    level=logging.INFO)
logger = logging.getLogger(__name__)

# Create the Updater and pass it your bot's TOKEN.
# Make sure to set use_context=True to use the new context based callbacks
updater = Updater(TOKEN, use_context=True)

dt_classifier = MyDecisionTreeClassifier()

# Pre-load all detection modules
detection_modules = {
    "nltk": importlib.import_module('detection_modules.nltk_vishing_detection'),
    "cosineSim": importlib.import_module('detection_modules.cosineSim_vishing_detection'),
    "gensim": importlib.import_module('detection_modules.gensim_vishing_detection'),
    "kmp": importlib.import_module('detection_modules.kmp_vishing_detection'),
    "sklearn": importlib.import_module('detection_modules.sklearn_vishing_detection'),
    "spacy": importlib.import_module('detection_modules.spacy_vishing_detection'),
    "trie": importlib.import_module('detection_modules.trie_vishing_detection'),
}

# Default vishing detection module
vishing_detector = detection_modules['trie']

QUIZ, END = range(2)
CANCEL = threading.Event()

def set_module(update: Update, context: CallbackContext) -> int:
    # Extract the method name from the module name string
    current_module = vishing_detector.__name__.split('.')[1].replace('_vishing_detection', '')
    
    keyboard = [
        [InlineKeyboardButton("CosineSim", callback_data='cosineSim'),
        InlineKeyboardButton("Gensim", callback_data='gensim')],
        [InlineKeyboardButton("KMP", callback_data='kmp'),
        InlineKeyboardButton("NLTK", callback_data='nltk')],
        [InlineKeyboardButton("Sklearn", callback_data='sklearn'),
        InlineKeyboardButton("Spacy", callback_data='spacy')],
        [InlineKeyboardButton("Trie", callback_data='trie')]
    ]

    reply_markup = InlineKeyboardMarkup(keyboard)

    update.message.reply_text(
        f'🔧 Current detection module is: {current_module}. \n'
        'Please choose a detection module from the menu below:', reply_markup=reply_markup
    )


def module_selection(update: Update, context: CallbackContext) -> None:
    query = update.callback_query
    query.answer()
    module_name = query.data

    update.effective_message.edit_text(f'Hang on... 🔄 Loading module: {module_name}...')
    global vishing_detector
    vishing_detector = detection_modules[module_name]
    update.effective_message.edit_text(f'All set! ✅ Detection module changed to: {module_name}')



def start(update: Update, context: CallbackContext) -> None:
    context.user_data['state'] = 'IDLE'

    # Extract the method name from the module name string
    current_module = vishing_detector.__name__.split('.')[1].replace('_vishing_detection', '')

    update.message.reply_text(
        f'👋 Hello there! Ready to test messages or audios for vishing? Just send them over! \n\n'
        f'🔧 We are currently using the {current_module} detection module. '
        'Want to try another module? Use /set_module to choose from the available modules.\n\n'
        '💡 Want to compare the performance of different detection modules? Use /compare to start the comparison. Comparisons can take a bit of time, so we have pre-prepared comparison results for you.\n\n'
        '📊 Simply use /compare_results to instantly get the results of our latest module comparisons.\n\n'
        '🔍 Feeling like a detective? Start a vishing detection quiz with /quiz. \n\n'
    )





def button(update: Update, context: CallbackContext) -> None:
    query = update.callback_query
    query.answer()

    if query.data == '/quiz':
        quiz_command(update, context)

def quiz_command(update: Update, context: CallbackContext) -> int:
    context.user_data['state'] = 'QUIZ'  
    context.bot.send_message(chat_id=update.effective_chat.id, text="Starting vishing detection quiz...")
    dt_classifier.start_quiz(update, context)
    return QUIZ

def quiz_response(update: Update, context: CallbackContext) -> int:
    dt_classifier.collect_response(update, context)
    if dt_classifier.is_quiz_complete():
        context.user_data['state'] = 'IDLE'  
        reply_markup = ReplyKeyboardRemove()  
        context.bot.send_message(chat_id=update.effective_chat.id, text="Quiz completed. Analyzing responses...", reply_markup=reply_markup)
        result_1 = dt_classifier.analyze_responses()
        context.bot.send_message(chat_id=update.effective_chat.id, text=result_1, parse_mode='Markdown')
        return END
    else:
        return QUIZ

def end(update: Update, context: CallbackContext) -> int:
    context.user_data['state'] = 'IDLE'
    return ConversationHandler.END

def format_reply(module_name, result_1, result_2, result_3):
    if module_name in ['detection_modules.kmp_vishing_detection', 'detection_modules.trie_vishing_detection']:
        if result_1 == "Vishing":
            return f'*Vishing Detected!* 🚨\nThe message has a *{result_2:.2f}%* likelihood of being a vishing attempt.\n\n*Type of Vishing:* {result_3}'
        else:
            return f'Good news! 🎉\nThe message seems safe with a low vishing likelihood of *{result_2:.2f}*. Always stay alert!'
    else:
        if result_2 > result_3:
            return f'*Vishing Detected!* 🚨\nThe message similarity to a vishing attempt is *{result_2:.2f}* and to a non-vishing content is *{result_3:.2f}*. Always stay alert!'
        else:
            return f'Good news! 🎉\nThe message seems safe. Its similarity to a vishing attempt is *{result_2:.2f}* and to a non-vishing content is *{result_3:.2f}*. Always stay alert!'

def handle_text(update: Update, context: CallbackContext) -> None:
    if context.user_data.get('state') == 'QUIZ':
        return

    user_text = update.message.text.lower()
    result_1, result_2, result_3 = vishing_detector.detect_vishing(user_text)
    reply_message = format_reply(vishing_detector.__name__, result_1, result_2, result_3)
    update.message.reply_text(reply_message, parse_mode='Markdown')

def handle_audio(update: Update, context: CallbackContext) -> None:
    file = context.bot.get_file(update.message.voice.file_id)
    file.download('voice.ogg')
    os.system("ffmpeg -y -i voice.ogg voice.wav")
    r = sr.Recognizer()
    with sr.AudioFile('voice.wav') as source:
        audio = r.record(source)
        user_text = r.recognize_google(audio, language='en-US')
        result_1, result_2, result_3 = vishing_detector.detect_vishing(user_text)
        reply_message = format_reply(vishing_detector.__name__, result_1, result_2, result_3)
        update.message.reply_text(reply_message, parse_mode='Markdown')

    try:
        os.remove('voice.ogg')
        os.remove('voice.wav')
    except Exception as e:
        print("Error deleting audio files: ", e)


quiz_handler = ConversationHandler(
    entry_points=[CommandHandler('quiz', quiz_command)],
    states={
        QUIZ: [MessageHandler(Filters.text & ~Filters.command, quiz_response)]
    },
    fallbacks=[CommandHandler('cancel', end)],
    conversation_timeout=300,
    map_to_parent={
        END: QUIZ,
    },
)

def load_detection_modules(path='detection_modules'):
    modules = {}
    for filename in os.listdir(path):
        if filename.endswith('.py') and filename != 'quiz_decision_tree.py':
            spec = importlib.util.spec_from_file_location(filename[:-3], os.path.join(path, filename))
            module = importlib.util.module_from_spec(spec)
            spec.loader.exec_module(module)
            modules[filename[:-3]] = module
    return modules

def send_cancel_button(update: Update, context: CallbackContext) -> None:
    cancel_keyboard = [['/cancel']]
    cancel_markup = ReplyKeyboardMarkup(cancel_keyboard, one_time_keyboard=True)
    update.message.reply_text('Press /cancel to stop the comparison at any time.', reply_markup=cancel_markup)

def remove_cancel_button(update: Update, context: CallbackContext) -> None:
    update.message.reply_text('Operation finished.', reply_markup=ReplyKeyboardRemove())


def compare_algorithms(update: Update, context: CallbackContext) -> None:
    send_cancel_button(update, context)
    
    update.message.reply_text('Loading detection modules...')
    
    # No need to load modules here, use the pre-loaded ones
    modules = detection_modules

    update.message.reply_text('Finished loading modules. Now processing...')
    
    # Load input data
    input_data = pd.read_csv('resources/test_cases.csv')

    processing_times = {module_name: [] for module_name in modules}
    preprocessing_times = {module_name: [] for module_name in modules}
    correct_predictions = {module_name: 0 for module_name in modules}
    total_predictions = {module_name: 0 for module_name in modules}

    for index, row in input_data.iterrows():
        if CANCEL.is_set():
            update.message.reply_text("Comparison cancelled.")
            CANCEL.clear()
            return
        
        transcript = row['Transcript']
        actual_label = row['Label']

        update.message.reply_text(
            f'*Test Case Index: {index}*\n'
            f'*Transcript: {transcript}*\n'
            f'*Actual Label: {actual_label}*\n',
            parse_mode='Markdown'
        )
        
        for module_name, module in modules.items():
            module_name_short = module_name.replace('_vishing_detection', '')
            # Send processing status update	
            status_message = context.bot.send_message(chat_id=update.effective_chat.id, text=f"Processing with module: {module_name_short}...")	
            start = time.time()	
            result = module.detect_vishing(transcript)	
            end = time.time()	
            # Delete status message	
            context.bot.delete_message(chat_id=update.effective_chat.id, message_id=status_message.message_id)
            predicted_label = result[0]

            update.message.reply_text(
                f'Module: {module_name_short}\n'
                f'Predicted label: {predicted_label}\n'
                f'Full Results: {result}\n'
                f'Time taken: {end - start}s\n'
            )
            processing_times[module_name].append(end - start)
            preprocessing_times[module_name].append(module.PREPROCESSING_TIME)

            # Update correct predictions and total predictions
            total_predictions[module_name] += 1
            if predicted_label == actual_label:
                correct_predictions[module_name] += 1
                
    update.message.reply_text(f'*Comparison completed! \nOverall Conclusion Below*',parse_mode='Markdown')

    # Calculate and display accuracy, average processing time, and average preprocessing time for each module
    for module_name in modules:
        avg_processing_time = sum(processing_times[module_name])/len(processing_times[module_name])
        avg_preprocessing_time = sum(preprocessing_times[module_name])/len(preprocessing_times[module_name])
        accuracy = correct_predictions[module_name] / total_predictions[module_name]
        update.message.reply_text(
            f'Module: {module_name_short}\n'
            f'Accuracy: {(accuracy*100):.1f}%\n'
            f'Preprocessing time: {avg_preprocessing_time}s\n'
            f'Average processing time: {avg_processing_time}s\n'
        )

    remove_cancel_button(update, context)

def cancel(update: Update, context: CallbackContext) -> None:
    CANCEL.set()
    update.message.reply_text(f'*Cancellation requested. This will take effect after the current test case is finished.*', parse_mode='Markdown')
    remove_cancel_button(update, context)

def compare_results(update: Update, context: CallbackContext) -> None:
    try:
        with open('resources/compare_results.txt', 'rb') as file:
            context.bot.send_document(chat_id=update.effective_chat.id, document=file)
    except FileNotFoundError:
        update.message.reply_text('File not found. Please ensure the comparison has been executed and the results file exists.')
    except Exception as e:
        update.message.reply_text(f'Error sending file: {str(e)}')


# Handler addition for /compare_results
updater.dispatcher.add_handler(CommandHandler('compare_results', compare_results))


# Handler additions
updater.dispatcher.add_handler(CommandHandler('compare', lambda update, context: threading.Thread(target=compare_algorithms, args=(update, context)).start()))
updater.dispatcher.add_handler(CommandHandler('cancel', cancel))

updater.dispatcher.add_handler(CommandHandler('set_module', set_module))
updater.dispatcher.add_handler(CommandHandler('start', start))
updater.dispatcher.add_handler(quiz_handler)
updater.dispatcher.add_handler(MessageHandler(Filters.text & ~Filters.command, handle_text))
updater.dispatcher.add_handler(MessageHandler(Filters.voice, handle_audio))
updater.dispatcher.add_handler(CallbackQueryHandler(module_selection))

# Start the Bot
updater.start_webhook(listen="0.0.0.0",port=os.environ.get("PORT",443),
                        url_path=TOKEN,
                        webhook_url="https://vishing-detector-bot-6102c3800103.herokuapp.com/"+TOKEN)

updater.idle()