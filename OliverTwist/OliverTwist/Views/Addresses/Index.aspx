<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.AddressModel, OliverTwist.FilterContainers.AddressFilterContainer>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
	Ваша адресная книга
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.HiddenFor(model => model.FilterContainer.SelectedGroups, new { @class = "jquery_selectedGroups" }) %>
    <script src="<%= Url.Content("~/Scripts/jquery.jstree.js")%>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.pop.js")%>" type="text/javascript"></script>
    <link href="<%= Url.Content("~/Content/pop.css")%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var selectedGroupsStorage = $(".jquery_selectedGroups")[0];
        $.jstree._themes = "<%= Url.Content("~/Content/jstree_themes/")%>";
        $(document).ready(function(){
        $('#groups').jstree({
           plugins  :  [ "themes", "json_data", "ui", "checkbox", "crrm", "dnd", "contextmenu"],
           json_data : {
                         data : [{  
                                    data  : "Все группы",  
                                    attr  : { Id : 0 },
                                    state : "closed"
                                  }],
                         ajax : {
                                    type     : "POST", 
                                    dataType : "json",
                                    url      : "<%= Url.Action("GetGroups") %>",
                                    data     : function (node) {  
                                                                   return { 
                                                                            id : node.attr ? node.attr("id") : 0 
                                                                          };
                                                 }
                                 }
                        },
            ui :  {
                    select_limit : -1,
                    select_multiple_modifier : "ctrl",
                    selected_parent_close : "select_parent",
                    initially_select : $.parseJSON(selectedGroupsStorage.value.replace(/[\[\]\s]/g, "") == "" ? "[\"0\"]" : selectedGroupsStorage.value)
                   },
            core : { 
	                  strings: 
                             {
                               loading  : "Загрузка ...",
                               new_node : "Новая группа"
                             },
                      initially_open : [ 0 ] 
	               },
            contextmenu :
                            {
                               items: 
                                        {
                                            ccp    : false,
                                            create : {
                                                         label     : "Создать",
                                                         action    : function (obj) { this.create(obj); }
                                                     },
                                            rename : {	                                        
                                                        label     : "Переименовать",
                                                        action    : function (obj) { this.rename(obj); } 
                                                     },
                                            remove : {	                                        
                                                        label     : "Удалить",
                                                        action    : function (obj) { this.remove(obj); } 
                                                     }
                                        } 
                            }

       })
        .bind("change_state.jstree", function (event, data) {
            var selectedNodes = $.map(data.inst.get_selected(),
                function(item, index)
                {
                    return $(item).attr("id");
                });
             selectedGroupsStorage.value = JSON.stringify(selectedNodes);
             var pop = $(this).closest(".pop");
             var tree = $(this);
             if (selectedNodes.length == 0)
             {
                data.inst.check_node(tree.find("#0"));
                return;
             }
             pop.nextAll(".selectedGroup").remove();
             data.inst.get_selected().each(function()
                {
                    var selectedBlock = $("<span class='selectedGroup'>"+$(this).children()[1].text+"</span>");
                    if ($(this).attr("id") != "0")
                    {
                        var closeIcon = $("<a onclick = 'dropSelectedGroup(\""+$(this).attr("id")+"\", \""+tree.attr("id")+"\")' style='cursor: pointer; margin-left: 2px;' >x</a>");
                        selectedBlock.append(closeIcon);
                    }
                    pop.after(selectedBlock);
                }
             );
             pop.next().first().css('margin-left', '25px');
        })
        .bind("close_node.jstree", function (event, data) {
            var item = $(data.args[0]);
            if (item.attr("id") == "0")
            {
                item.removeClass("jstree-closed");
                item.addClass("jstree-default");
                $(this).stop();                                
                event.cancelBubble = true;
                return false;
            }
        })
        .bind("create.jstree", function (e, data) {
			$.ajax({
				async : false,
                url   : "/static/v.1.0rc2/_demo/server.php", 
				data  : { 
					        operation : "create_node", 
					        id : data.rslt.parent.attr("id"), 
					        position : data.rslt.position,
					        title : data.rslt.name
				        },
                success : function (r) {
					                        if(r.status) {
                                    						$(data.rslt.obj).attr("id", r.id);
					                                     }
					                        else {
						                            $.jstree.rollback(data.rlbk);
					                             }
				                       },
                error   : function (r) {
                                            $.jstree.rollback(data.rlbk);
                                       }
			});
		})
		.bind("remove.jstree", function (e, data) {
			data.rslt.obj.each(function () {
				$.ajax({
					async : false,
					type: "POST",
					url: "/static/v.1.0rc2/_demo/server.php",
					data : { 
						operation : "remove_node", 
						id : this.id.replace("node_","")
					}, 
					success : function (r) {
						if(!r.status) {
							data.inst.refresh();
						}
					},
                    error   : function (r) {
							data.inst.refresh();
						} 
				});
			});
		})
		.bind("rename.jstree", function (e, data) {
			$.ajax({
				async : false,
                type  : "POST",
                url   : "/static/v.1.0rc2/_demo/server.php", 
                data  :	{ 
					        operation : "rename_node", 
					        id : data.rslt.obj.attr("id").replace("node_",""),
					        title : data.rslt.new_name
				        },
				success : function (r) {
					if(!r.status) {
						$.jstree.rollback(data.rlbk);
					}
				},
                error   : function(r)
                {
                   $.jstree.rollback(data.rlbk); 
                }
			});
		})
		.bind("move_node.jstree", function (e, data) {
			data.rslt.o.each(function (i) {
				$.ajax({
					async : false,
					type: "POST",
					url: "/static/v.1.0rc2/_demo/server.php",
					data : { 
						operation : "move_node", 
						id : $(this).attr("id").replace("node_",""), 
						ref : data.rslt.np.attr("id").replace("node_",""), 
						position : data.rslt.cp + i,
						title : data.rslt.name,
						copy : data.rslt.cy ? 1 : 0
					},
					success : function (r) {
						if(!r.status) {
							$.jstree.rollback(data.rlbk);
						}
						else {
							$(data.rslt.oc).attr("id", "node_" + r.id);
							if(data.rslt.cy && $(data.rslt.oc).children("UL").length) {
								data.inst.refresh(data.inst._get_parent(data.rslt.oc));
							}
						}
					},
                    error  : function (r)
                            {
                               $.jstree.rollback(data.rlbk); 
                            }        
				});
			});
        });
        $.pop();
    });
    function dropSelectedGroup(groupId, treeId)
    {
        var treeNode = $("#"+treeId);
        treeNode.jstree("uncheck_node", treeNode.find("#"+groupId).first());    
    }
    </script>   
    <h2>Ваша адресная книга</h2>
    <% Html.RenderPartial("AddressSearchFilter", Model.FilterContainer); %>
        <span class="pop">
            <div id="groups"></div>
        </span>
        <% Html.RenderPartial("SearchResultsWithPaging", Model); %>
        <hr />
        <div>
        <%= Html.ActionLink("Создать адрес", "CreateNew")%>
        </div>
</asp:Content>
