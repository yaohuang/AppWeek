function todoItem(dataModel, data) {
    var self = this;
    data = data || {};

    // Persisted properties
    self.todoItemId = data.todoItemId;
    self.title = ko.observable(data.title);
    self.isDone = ko.observable(data.isDone);
    self.todoListId = data.todoListId;

    // Non-persisted properties
    self.errorMessage = ko.observable();

    function saveChanges() {
        self.errorMessage(null);
        return dataModel.saveChangedTodoItem(self)
            .fail(function () {
                self.errorMessage("Error updating todo item.");
            });
    }

    // Auto-save when these properties change
    self.isDone.subscribe(saveChanges);
    self.title.subscribe(saveChanges);

    self.toJson = function () { return ko.toJSON(self) };
}

function todoList(dataModel, data) {
    var self = this;
    data = data || {};

    // convert raw todoItem data objects into array of TodoItems
    function importTodoItems(todoItems) {
        /// <returns value="[new todoItem()]"></returns>
        return $.map(todoItems || [],
                function (todoItemData) {
                    return new todoItem(dataModel, todoItemData);
                });
    }

    // Persisted properties
    self.MeetingPlace = ko.observable(data.MeetingPlace || "to be replaced..");
    self.Id = data.Id;
    self.Subject = ko.observable(data.Subject || "Some subject..");
    self.Users = ko.observable(data.Users || ['Yao', 'Gustavo', 'Luke', 'Raghu']);
    //self.userId = data.userId || "to be replaced";
    //self.title = ko.observable(data.title || "My todos");
    //self.todos = ko.observableArray(importTodoItems(data.todos));

    // Non-persisted properties
    //self.isEditingListTitle = ko.observable(false);
    self.newTodoTitle = ko.observable();
    self.errorMessage = ko.observable();

    self.addTodo = function () {
        if (self.newTodoTitle()) { // need a title to save
            var item = new todoItem(
                dataModel,
                {
                    title: self.newTodoTitle(),
                    todoListId: self.todoListId
                });
            self.todos.push(item);
            item.errorMessage(null);
            dataModel.saveNewTodoItem(item)
                .done(function (result) {
                    item.todoItemId = result.todoItemId;
                })
                .fail(function () {
                    item.errorMessage("Error adding a new todo item.");
                });
            self.newTodoTitle("");
        }
    };

    self.deleteTodo = function () {
        var todoItem = this;
        return dataModel.deleteTodoItem(todoItem)
            .done(function () {
                self.todos.remove(todoItem);
            })
            .fail(function () {
                todoItem.errorMessage("Error removing todo item.");
            });
    };

     //Auto-save when these properties change
    self.MeetingPlace.subscribe(function () {
        self.errorMessage(null);
        return dataModel.saveChangedTodoList(self)
            .fail(function () {
                self.errorMessage("Error updating the lunch meeting place.");
            });
    });
    self.Subject.subscribe(function () {
        self.errorMessage(null);
        return dataModel.saveChangedTodoList(self)
            .fail(function () {
                self.errorMessage("Error updating the lunch subject.");
            });
    });

    self.toJson = function () { return ko.toJSON({ MeetingPlace : self.MeetingPlace, Subject : self.Subject }) };
}
