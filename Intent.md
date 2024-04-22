# Intent Classification / Recognition
Intent classification, particularly in the context of natural language processing (NLP) and conversational AI, has been a topic of interest for several decades, but it gained significant attention with the rise of chatbots and virtual assistants.

The roots of intent classification can be traced back to the early days of AI research, with early attempts at understanding user intent in dialogue systems. However, it wasn't until the late 20th and early 21st centuries, with advancements in machine learning and NLP techniques, that intent classification became a more prominent area of study.

The proliferation of messaging platforms and virtual assistants in the past decade has further fueled research and development in intent classification. Today, it's a crucial component of various applications, including chatbots, virtual assistants, customer service automation, and more.

So, while intent classification has been a topic in the AI and ML space for quite some time, its significance and relevance have grown substantially in recent years with the increasing demand for natural and intuitive human-computer interactions.

## Great Article on Intent Classification
I would highly recommending [read this article](https://medium.com/aimonks/intent-classification-generative-ai-based-application-architecture-3-79d2927537b4) which covers the topic very well.

## Quorum Approach
I have implemented a [static intent.cs](./api-ai-rag-intent/Util/Intent.cs) class that takes advantage of ResultsPerPrompt which allows us to use a Quorum approach.  By setting ResultsPerPrompt to 3 this allows the model to leverage the multiple classifiers to predict the intent of the user input to arrive at a final decision.  The result is a much are accurate way to perform intent classification as you can now use the results of the quorum to arrive at a final decision.

The concept of using a quorum to detect intent in AI is akin to ensemble learning techniques, where multiple models or components are used to make a collective decision. In the context of intent detection, a quorum approach involves employing multiple classifiers or models to predict the intent of a user input, and then combining their predictions to arrive at a final decision.

Using a quorum approach can enhance the robustness and reliability of intent detection systems by reducing the impact of individual classifier errors or biases. It's particularly useful in situations where a single classifier may struggle due to ambiguity in the user input or variations in language use.

