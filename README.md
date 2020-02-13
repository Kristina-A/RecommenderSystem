# RecommenderSystem
Web shop application created in ASP.NET MVC 5 and personalized using user activities and recommender system. For recommender system user-user and item-item collaborative methods were used. MongoDB and TimescaleDB were used as databases. In MongoDB information about users and products were stored, and TimescaleDB was used for storing user activities (viewed product, bought product, reviewed product, saw product reviews, received notification).

# Personalized parts of application
Some parts of application are pesonalized for each user. For some of them are used only user activities, and for some are used recommender system methods.

### Recommended products on home page
- For products recommendation combination of user-user and item-item collaborative methods is used

![Untitled](https://user-images.githubusercontent.com/37186937/74422120-60ff9680-4e4e-11ea-8cb1-a8c58013eed3.png)

### Recommended ads on home page
- For ads recommendation combination of user-user and item-item collaborative methods is used

![ads](https://user-images.githubusercontent.com/37186937/74422219-8ee4db00-4e4e-11ea-905e-22241424ef81.png)

### Last seen
- List of products user has viewed recently. Activities (viewed product) form TimescaleDB are used.

### Search recommendations
- For search recommendation item-item collaborative method is used

### Users who bought this product also bought
- This part was implemented using user's activities from TimescaleDB (bought product activities)

### Notifications
- This part was implemented using user's activities from TimescaleDB (bought product, reviewed product and received notification activities)
