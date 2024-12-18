from transformers import MarianMTModel, MarianTokenizer
import sys
import json

def translate_text(text, source_lang, target_lang):
    try:
        model_name = f'Helsinki-NLP/opus-mt-{source_lang}-{target_lang}'
        tokenizer = MarianTokenizer.from_pretrained(model_name)
        model = MarianMTModel.from_pretrained(model_name)
        
        inputs = tokenizer(text, return_tensors="pt", padding=True)
        translated = model.generate(**inputs)
        translated_text = tokenizer.batch_decode(translated, skip_special_tokens=True)[0]
        
        return json.dumps({"success": True, "translated_text": translated_text})
    except Exception as e:
        return json.dumps({"success": False, "error": str(e)})

if __name__ == "__main__":
    if len(sys.argv) != 4:
        print(json.dumps({"success": False, "error": "Invalid arguments. Expected: text source_lang target_lang"}))
    else:
        text = sys.argv[1]
        source_lang = sys.argv[2]
        target_lang = sys.argv[3]
        print(translate_text(text, source_lang, target_lang))
