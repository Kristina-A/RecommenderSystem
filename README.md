# RecommenderSystem
Web shop application created in ASP.NET MVC 5 and personalized using user activities and recommender system. For recommender system user-user and item-item collaborative methods were used. MongoDB and TimescaleDB were used as databases. In MongoDB information about users and products were stored, and TimescaleDB was used for storing user activities (viewed product, bought product, reviewed product, saw product reviews, received notification).

# Personalized parts of application
Some parts of application are pesonalized for each user. For some of them are used only user activities, and for some are used recommender system methods.

### Recommended products on home page
- For products recommendation combination of user-user and item-item collaborative methods is used

### Recommended ads on home page
- For ads recommendation combination of user-user and item-item collaborative methods is used

### Search recommendations
- For search recommendation item-item collaborative method is used

### Users who bought this product also bought
- This part was implemented using user's activities from TimescaleDB (bought product activities)

### Notifications
- This part was implemented using user's activities from TimescaleDB (bought product, reviewed product and received notification activities)
