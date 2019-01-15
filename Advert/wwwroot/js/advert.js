function logInByEmailAsync(login, headers, success, error) {
    console.log(`Log in with email: ${login.email} password: ${login.password}`);
    $.ajax({
        url: `/Account/LogInByEmail?returnUrl=${window.location.pathname}`,
        type: "POST",
        data: JSON.stringify(login),
        headers: headers,
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });

}

function createNewAccountAsync(account, headers, success, error) {
    console.log(`Create new account with name: ${account.name} email: ${account.email} password: ${account.password}`);
    $.ajax({
        url: `/Account/CreateNewAccount?returnUrl=${window.location.pathname}`,
        type: "POST",
        data: JSON.stringify(account),
        headers: headers,
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function createPostAsync(post, success, error) {
    console.log(`Create post title: ${post.title} text: ${post.text} price: ${post.price}${post.currency} location: ${post.location}`);
    $.ajax({
        url: "/Posts/CreatePostAsync",
        type: "POST",
        data: JSON.stringify(post),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function getPostByIdAsync(postId, success, error) {
    console.log(`Get post by id: ${postId}`);
    $.ajax({
        url: `/Posts/GetPostByIdAsync?postId=${postId}`,
        type: "GET",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function getPostCommentAsync(postId, commentId, success, error) {
    console.log(`Get post comment by post id: ${postId} comment id: ${commentId}`);
    $.ajax({
        url: `/Posts/getPostCommentAsync?postId=${postId}&commentId=${commentId}`,
        type: "GET",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function updatePostByIdAsync(postId, post, success, error) {
    console.log(`Update post by id: ${postId}`);
    $.ajax({
        url: `/Posts/UpdatePostByIdAsync?postId=${postId}`,
        type: "POST",
        data: JSON.stringify(post),
        contentType: "application/json",
        error: function() {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function updatePostCommentAsync(postId, commentId, postComment, success, error) {
    console.log(`Update post comment by post id: ${postId} comment id: ${commentId}`);
    $.ajax({
        url: `/Posts/UpdatePostCommentAsync?postId=${postId}&commentId=${commentId}`,
        type: "POST",
        data: JSON.stringify(postComment),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function deletePostByIdAsync(postId, success, error) {
    console.log(`Delete post by id: ${postId}`);
    $.ajax({
        url: `/Posts/DeletePostByIdAsync?postId=${postId}`,
        type: "DELETE",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function deletePostCommentAsync(postId, commentId, success, error) {
    console.log(`Delete post comment by post id: ${postId} comment id: ${commentId}`);
    $.ajax({
        url: `/Posts/DeletePostCommentAsync?postId=${postId}&commentId=${commentId}`,
        type: "DELETE",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    }); 
}

function createPostCommentAsync(comment, success, error) {
    $.ajax({
        url: `/Posts/CreatePostCommentAsync`,
        type: "POST",
        data: JSON.stringify(comment),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function countPostCommentsByPostIdAsync(postId, success, error) {
    $.ajax({
        url: `/Posts/CountPostCommentsByPostIdAsync?postId={postId}`,
        type: "GET",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function createPostVoteIdAsync(postId, vote, success, error) {
    console.log(`Create post vote by post id: ${postId} vote type: ${vote.voteType}`);
    $.ajax({
        url: `/Posts/CreatePostVoteAsync?postId=${postId}`,
        type: "POST",
        data: JSON.stringify(vote),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function createPostCommentVoteAsync(postId, commentId, vote, success, error) {
    $.ajax({
        url: `/Posts/CreatePostCommentVoteAsync?postId=${postId}&commentId=${commentId}`,
        type: "POST",
        data: JSON.stringify(vote),
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function getPostTagsByPostIdAsync(postId, success, error) {
    console.log(`Get post tags by post id: ${postId}`);
    $.ajax({
        url: `/Posts/GetPostTagsByPostIdAsync?postId=${postId}`,
        type: "GET",
        contentType: "application/json",
        error: function(jqXhr, textStatus, errorThrown) {
            console.log("jqXhr: " + jqXhr);
            console.log("textStatus: " + textStatus);
            console.log("errorThrown: " + errorThrown);
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

function createPostTagsByPostIdAsync(postId, postTags, success, error) {
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
            error(jqXhr, textStatus, errorThrown);
        },
        success: function(response) {
            success(response);
        }
    });
}

