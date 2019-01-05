function createPostAsync(post, callback) {
    console.log(`Create post title: ${post.title} text: ${post.text} price: ${post.price}${post.currency} location: ${post.location}`);
    $.ajax({
        url: "/Posts/CreatePostAsync",
        type: "POST",
        data: JSON.stringify(post),
        contentType: "application/json",
        error: function() {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function getPostByIdAsync(postId, callback) {
    console.log(`Get post by id: ${postId}`);
    $.ajax({
        url: `/Posts/GetPostByIdAsync?postId=${postId}`,
        type: "GET",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function updatePostByIdAsync(postId, post, callback) {
    console.log(`Update post title: ${post.title} text: ${post.text} price: ${post.price}${post.currency} location: ${post.location} by id: ${postId}`);
    $.ajax({
        url: `/Posts/UpdatePostByIdAsync?postId=${postId}`,
        type: "POST",
        data: JSON.stringify(post),
        contentType: "application/json",
        error: function() {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function deletePostByIdAsync(postId, callback) {
    console.log(`Delete post by id: ${postId}`);
    $.ajax({
        url: `/Posts/DeletePostByIdAsync?postId=${postId}`,
        type: "DELETE",
        contentType: "application/json",
        error: function() {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function createPostCommentAsync(comment, callback) {
    $.ajax({
        url: `/Posts/CreatePostCommentAsync`,
        type: "POST",
        data: JSON.stringify(comment),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function countPostCommentsByPostIdAsync(postId, callback) {
    $.ajax({
        url: `/Posts/CountPostCommentsByPostIdAsync?postId={postId}`,
        type: "GET",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function createPostVoteIdAsync(postId, vote, callback) {
    console.log(`Create post vote byte post id: ${postId} vote type: ${vote.voteType}`);
    $.ajax({
        url: `/Posts/CreatePostVoteAsync?postId=${postId}`,
        type: "POST",
        data: JSON.stringify(vote),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function createPostCommentVoteAsync(postId, commentId, vote, callback) {
    $.ajax({
        url: `/Posts/CreatePostCommentVoteAsync?postId=${postId}&commentId=${commentId}`,
        type: "POST",
        data: JSON.stringify(vote),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function getPostTagsByPostIdAsync(postId, callback) {
    console.log(`Get post tags by post id: ${postId}`);
    $.ajax({
        url: `/Posts/GetPostTagsByPostIdAsync?postId=${postId}`,
        type: "GET",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

function createPostTagsByPostIdAsync(postId, postTags, callback) {
    console.log(`Create post tags: ${postTags.tags}`);
    $.ajax({
        url: `/Posts/CreatePostTagsByPostIdAsync?postId=${postId}`,
        type: "POST",
        data: JSON.stringify(postTags),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
        },
        success: function(response) {
            callback(response);
        }
    });
}

